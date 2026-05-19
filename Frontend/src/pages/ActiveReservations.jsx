import React, { useEffect, useState, useCallback, useMemo } from 'react'
import api from '../utils/axios'
import ActiveReservationCard from '../components/ActiveReservationCard'
import VerificationNotification from '../components/VerificationNotification'
import { useTranslation } from 'react-i18next'

const ActiveReservations = () => {
  const { t } = useTranslation()
  const [reservations, setReservations] = useState([])
  const [loading, setLoading] = useState(true)
  const [showVerificationVisible, setShowVerificationVisible] = useState(false)
  const [message, setMessage] = useState("")
  const [onConfirm, setOnConfirm] = useState(null)

  const getMyReservations = async () => {
    try {
      const { data } = await api.get('reservations/get-by-current-user')
      if (data.success) setReservations(data.data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  const handleReservationUpdate = useCallback(() => {
    getMyReservations()
  }, [])

  const branchIds = useMemo(
    () => [...new Set(reservations.map(r => r.branchId))],
    [reservations]
  )


  useEffect(() => {
    getMyReservations()
  }, [])

  const formatDate = (dateString) => {
    if (!dateString) return null

    const date = new Date(dateString)

    const day = String(date.getDate()).padStart(2, "0")
    const month = String(date.getMonth() + 1).padStart(2, "0")
    const year = date.getFullYear()

    const hours = String(date.getHours()).padStart(2, "0")
    const minutes = String(date.getMinutes()).padStart(2, "0")

    return `${day}.${month}.${year} ${hours}:${minutes}`
  }

  if (loading) return (
    <div className="h-screen justify-center items-center flex w-full mt-20 text-center text-gray-500">
      {t('activeReservations.loading')}
    </div>
  )

  if (!reservations || reservations.length === 0) return (
    <div className="h-screen justify-center items-center flex w-full mt-20 text-center text-gray-500">
      {t('activeReservations.noReservations')}
    </div>
  )

  return (
    <div className="w-full min-h-screen bg-gray-900 p-4 space-y-4 mt-12">
      <h3 className='text-white text-4xl font-bold mt-10 text-center'>
        {t('activeReservations.header')}
      </h3>
      {showVerificationVisible && (
        <VerificationNotification
          message={message}
          setShowVerificationVisible={setShowVerificationVisible}
          onConfirm={onConfirm}
        />
      )}

      {reservations.map(r => {
        const formattedArrival = formatDate(r.estimatedArrivalTime)

        return (
          <ActiveReservationCard
            key={r.id}
            reservation={r}
            estimatedArrivalFormatted={formattedArrival}
            setShowVerificationVisible={setShowVerificationVisible}
            setMessage={setMessage}
            setOnConfirm={setOnConfirm}
            refreshReservations={getMyReservations}
          />
        )
      })}
    </div>
  )
}

export default ActiveReservations