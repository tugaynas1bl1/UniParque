import React, { useState } from 'react'
import toast from 'react-hot-toast'
import api from '../utils/axios'
import { useTranslation } from 'react-i18next'

const ActiveReservationCard = ({
  reservation,
  setShowVerificationVisible,
  setMessage,
  setOnConfirm,
  refreshReservations
}) => {
  const { t } = useTranslation()
  const [isHovered, setIsHovered] = useState(false)

  const formatDate = (dateString) => {
    if (!dateString) return null
    const d = new Date(dateString)
    const day = String(d.getDate()).padStart(2, "0")
    const month = String(d.getMonth() + 1).padStart(2, "0")
    const year = d.getFullYear()
    const hour = String(d.getHours()).padStart(2, "0")
    const min = String(d.getMinutes()).padStart(2, "0")
    return `${day}.${month}.${year} ${hour}:${min}`
  }

  const getStatusStyle = (status) => {
    switch (status?.toLowerCase()) {
      case "active":
        return "bg-green-500/20 text-green-400 border border-green-500/30"
      case "checkedin":
        return "bg-blue-500/20 text-blue-400 border border-blue-500/30"
      default:
        return "bg-gray-500/20 text-gray-400 border border-gray-500/30"
    }
  }

  const cancelReservation = async () => {
    try {
      const { data } = await api.delete(`reservations/cancel-reservation/${reservation.id}`)
      if (data.success) {
        toast.success(t('activeReservations.toastCancelled'))
        refreshReservations()
      }
    } catch {
      toast.error(t('activeReservations.toastError'))
    }
  }

  const handleCheckIn = () => {
    setMessage(t('activeReservations.confirmCheckIn'))
    setOnConfirm(() => () => checkIn())
    setShowVerificationVisible(true)
  }

  const checkIn = async () => {
    try {
      const { data } = await api.put(`/reservations/confirm-arrival/${reservation.id}`)
      if (data.success) {
        toast.success(t('activeReservations.toastWelcome'))
        setShowVerificationVisible(false)
        refreshReservations()
      }
    } catch {
      toast.error(t('activeReservations.toastError'))
    }
  }

  const handleEndSession = () => {
    setMessage(t('activeReservations.confirmEndSession'))
    setOnConfirm(() => () => endSession())
    setShowVerificationVisible(true)
  }

  const endSession = async () => {
    try {
      const { data } = await api.delete(`/reservations/end-session/${reservation.id}`)
      if (data.success) {
        toast.success(t('activeReservations.toastSessionEnded'))
        setShowVerificationVisible(false)
        refreshReservations()
      }
    } catch {
      toast.error(t('activeReservations.toastError'))
    }
  }

  const arrival = formatDate(reservation.estimatedArrivalTime)

  return (
    <div className="w-full px-4 mt-10">
      <div
        onMouseEnter={() => setIsHovered(true)}
        onMouseLeave={() => setIsHovered(false)}
        className="cursor-pointer bg-[#111827] border border-gray-700 rounded-xl p-5 transition-all duration-300 hover:border-gray-500"
      >
        
        <div className="flex justify-between items-start mb-4">
          <div>
            <h2 className="text-white text-lg font-semibold tracking-wide">
              {reservation.place}
            </h2>
            <p className="text-gray-400 text-md mt-1">
              {reservation.branch}
            </p>
          </div>

          <div className="flex flex-col items-end gap-2">
            <span className={`text-xs px-3 py-1 rounded-full ${getStatusStyle(reservation.status)}`}>
              {reservation.status}
            </span>
            <p className="text-lg font-bold text-green-400">
              {reservation.price} ₼
            </p>
          </div>
        </div>

        <div className="grid grid-cols-2 gap-y-3 gap-x-6 text-sm">
          <div>
            <p className="text-gray-500 text-xs">{t('activeReservations.section')}</p>
            <p className="text-white">{reservation.section}</p>
          </div>

          <div>
            <p className="text-gray-500 text-xs">{t('activeReservations.subSection')}</p>
            <p className="text-white">{reservation.subSection}</p>
          </div>

          <div>
            <p className="text-gray-500 text-xs">{t('activeReservations.car')}</p>
            <p className="text-white">{reservation.carNumber}</p>
          </div>

          {arrival && (
            <div>
              <p className="text-gray-500 text-xs">{t('activeReservations.arrival')}</p>
              <p className="text-white">{arrival}</p>
            </div>
          )}
        </div>

        <div className={`flex justify-end gap-3 mt-5 transition-all duration-300  ${isHovered ? "opacity-100" : "sm:opacity-100 lg:opacity-0"}`}>
          {reservation.status?.toLowerCase() === "active" && (
            <button
              onClick={handleCheckIn}
              className="bg-green-500 px-4 py-2 rounded-md text-sm font-medium hover:bg-green-400 transition"
            >
              {t('activeReservations.checkIn')}
            </button>
          )}

          {reservation.status?.toLowerCase() === "active" && (
            <button
              onClick={cancelReservation}
              className="bg-red-500 px-4 py-2 rounded-md text-sm font-medium hover:bg-red-400 transition"
            >
              {t('activeReservations.cancel')}
            </button>
          )}

          {reservation.status?.toLowerCase() === "checkedin" && (
            <button
              onClick={handleEndSession}
              className="bg-blue-500 px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-400 transition"
            >
              {t('activeReservations.end')}
            </button>
          )}
        </div>
      </div>
    </div>
  )
}

export default ActiveReservationCard