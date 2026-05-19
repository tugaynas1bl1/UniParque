import React, { useEffect, useState } from "react";
import api from "../utils/axios";
import Loader from "../components/Loader";

const AdminDashboard = () => {
  const [reservations, setReservations] = useState([]);
  const [connection, setConnection] = useState(null);
  const [userCount, setUserCount] = useState(null);
  const [activeReservationsCount, setActiveReservationsCount] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const [hasNext, setHasNext] = useState(false);
  const [hasPrevious, setHasPrevious] = useState(false);

  const [page, setPage] = useState(1);
  const pageSize = 10;

  const formatDate = (dateString) => {
    if (!dateString) return null;
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  const getReservations = async () => {
    try {
      setIsLoading(true);
      const { data } = await api.get(
        `reservations?Page=${page}&Size=${pageSize}`
      );
      if (data.success) {
        setReservations(data.data.items);
        setHasNext(data.data.hasNext);
        setHasPrevious(data.data.hasPrevious);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    getReservations();
  }, [page]);

  const getUsersCount = async () => {
    try {
      setIsLoading(true);
      const { data } = await api.get("auth/users-count");
      if (data.success) setUserCount(data.data);
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  const getActiveReservationsCount = async () => {
    try {
      setIsLoading(true);
      const { data } = await api.get("reservations/count-active");
      if (data.success) setActiveReservationsCount(data.data);
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    getUsersCount();
    getActiveReservationsCount();
  }, []);

  useEffect(() => {
    getReservations();
  }, [connection]);

  return (
    <div className="flex min-h-screen bg-gray-950 text-white">
      <div className="flex-1 p-4 sm:p-6 md:p-8">

        <div className="flex justify-between items-center mb-10">
          <h2 className="text-2xl sm:text-2xl mt-6 md:text-3xl font-bold">Dashboard</h2>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-10">
          <div className="bg-[#11161D] p-4 px-auto sm:p-6 rounded-2xl shadow-lg">
            <p className="text-gray-400">Total Users</p>
            {isLoading ? (
              <Loader />
            ) : (
              <h3 className="text-2xl sm:text-2xl md:text-2xl font-bold mt-2">{userCount}</h3>
            )}
          </div>

          <div className="bg-[#11161D] p-4 sm:p-6 rounded-2xl shadow-lg">
            <p className="text-gray-400">Active Reservations</p>
            {isLoading ? (
              <Loader />
            ) : (
              <h3 className="text-2xl sm:text-2xl md:text-2xl font-bold mt-2">{activeReservationsCount}</h3>
            )}
          </div>
        </div>

        <div className="hidden sm:hidden md:block bg-[#11161D] p-4 sm:p-6 rounded-2xl shadow-lg overflow-x-auto">
          <h3 className="text-lg md:text-xl font-semibold mb-4">Recent Reservations</h3>
          <table className="w-full text-left table-auto text-xs sm:text-sm md:text-base">
            <thead>
              <tr className="text-gray-400">
                <th>User</th>
                <th>Branch</th>
                <th>Section</th>
                <th>Subsection</th>
                <th>Place</th>
                <th>Car</th>
                <th>Created At</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {reservations.length > 0 ? (
                reservations.map((r) => (
                  <tr key={r.id} className="border-t border-white/10">
                    <td className="py-1 px-2">{r.user}</td>
                    <td className="py-1 px-2">{r.branch}</td>
                    <td className="py-1 px-2">{r.section}</td>
                    <td className="py-1 px-2">{r.subSection}</td>
                    <td className="py-1 px-2">{r.place}</td>
                    <td className="py-1 px-2">{r.carNumber}</td>
                    <td className="py-1 px-2">{formatDate(r.createdAt)}</td>
                    <td
                      className={`py-1 px-2 ${
                        r.status === "Active" || r.status === "Completed"
                          ? "text-green-500"
                          : r.status === "Expired" || r.status === "Cancelled"
                          ? "text-red-500"
                          : "text-blue-400"
                      }`}
                    >
                      {r.status}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-gray-400">
                    No reservations found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
          {isLoading && <Loader />}
        </div>

        <div className="md:hidden space-y-2">
          {reservations.length > 0 ? (
            reservations.map((r) => (
              <div key={r.id} className="bg-[#11161D] p-3 rounded-lg shadow-lg">
                <p><span className="text-gray-400">User:</span> {r.user}</p>
                <p><span className="text-gray-400">Branch:</span> {r.branch}</p>
                <p><span className="text-gray-400">Section:</span> {r.section}</p>
                <p><span className="text-gray-400">Subsection:</span> {r.subSection}</p>
                <p><span className="text-gray-400">Place:</span> {r.place}</p>
                <p><span className="text-gray-400">Car:</span> {r.carNumber}</p>
                <p><span className="text-gray-400">Created At:</span> {formatDate(r.createdAt)}</p>
                <p>
                  <span className="text-gray-400">Status:</span>{" "}
                  <span
                    className={`${
                      r.status === "Active" || r.status === "Completed"
                        ? "text-green-500"
                        : r.status === "Expired" || r.status === "Cancelled"
                        ? "text-red-500"
                        : "text-blue-400"
                    }`}
                  >
                    {r.status}
                  </span>
                </p>
              </div>
            ))
          ) : (
            <p className="text-center py-4 text-gray-400">No reservations found</p>
          )}
          {isLoading && <Loader />}
        </div>

        <div className="flex flex-col sm:flex-row justify-between items-center mt-6 gap-2 sm:gap-0">
          <button
            onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
            disabled={!hasPrevious}
            className={`${
              hasPrevious ? "" : "hidden"
            } px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600`}
          >
            Previous
          </button>
          <span className="text-gray-400">Page {page}</span>
          <button
            onClick={() => setPage((prev) => prev + 1)}
            disabled={!hasNext}
            className={`${
              hasNext ? "" : "hidden"
            } px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600`}
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;