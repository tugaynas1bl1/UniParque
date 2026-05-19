using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using UniParque.Application.Hubs;
using UniParque_Domain.Entities;
using UniParque_Infrastructure.Persistence;

namespace UniParque_Infrastructure.Services;

public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<ReservationHub> _hubContext;

    public ReservationCleanupService(IServiceScopeFactory scopeFactory, IHubContext<ReservationHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var expiredTask = RunExpiredCleanup(stoppingToken);
        var activeTask = RunActiveCleanup(stoppingToken);

        await Task.WhenAll(expiredTask, activeTask);
    }

    private async Task RunExpiredCleanup(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UniParqueDBContext>();

            var nowUtc = DateTime.UtcNow;

            var expiredReservations = await db.Reservations
                .Where(r => r.ReservationDeletionTime != null &&
                            r.ReservationDeletionTime <= nowUtc)
                .ToListAsync(stoppingToken);

            if (expiredReservations.Any())
            {
                db.Reservations.RemoveRange(expiredReservations);
                await db.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunActiveCleanup(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UniParqueDBContext>();

            var now = DateTime.UtcNow;

            var query = db.Reservations
            .Where(r => r.EstimatedArrivalTime != null &&
                r.EstimatedArrivalTime < now);

            var placeIds = await query
                .Select(r => r.PlaceId)
                .ToListAsync(stoppingToken);

            await query.ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, ReservationStatus.Expired),
                stoppingToken);

            if (placeIds.Any())
            {
                await db.ParkingPlaces
                    .Where(p => placeIds.Contains(p.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.IsTaken, false)
                        .SetProperty(p => p.IsReserved, false),
                        stoppingToken);
            }

            var expiredReservationIds = await query
            .Select(r => r.Id)
            .ToListAsync(stoppingToken);

            if (expiredReservationIds.Any())
            {
                await _hubContext.Clients.All.SendAsync(
                    "ReservationsExpired",
                    expiredReservationIds,
                    cancellationToken: stoppingToken
                );
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}

