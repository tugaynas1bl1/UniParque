import React, { useState, useRef, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

const Reservation = () => {
  const { t } = useTranslation()
  const [text, setText] = useState("+")
  const timeoutRef = useRef(null)

  const handleEnter = () => {
    timeoutRef.current = setTimeout(() => {
      setText(t('reservation.createButton'))
    }, 250)
  }

  const handleLeave = () => {
    clearTimeout(timeoutRef.current)
    setText("+")
  }

  useEffect(() => {
    document.body.style.overflow = "auto"
    window.scrollTo({ top: 0, behavior: "smooth" })
  }, [])

  return (
    <div className='min-h-screen bg-gradient-to-br from-[#F7F9FB] to-[#E9EEF3] flex flex-col overflow-x-hidden'>

      <div className='px-4 sm:px-8 md:px-16 lg:px-32 pt-20 pb-6 sm:pb-8 md:pb-10 animate-[comeFromTop_1s]'>
        <h1 className='text-2xl sm:text-3xl md:text-[32px] lg:text-[36px] font-semibold text-[#172A39]'>
          {t('reservation.header')}
        </h1>
        <p className='text-[#6B7A8C] mt-2 text-sm sm:text-[15px] md:text-[16px]'>
          {t('reservation.subHeader')}
        </p>
      </div>

      <div className='px-4 sm:px-8 md:px-16 lg:px-32 flex flex-col gap-4 sm:gap-5 md:gap-6'>
        <Link to='/my-active-reservations' className='bg-white rounded-2xl p-4 sm:p-5 md:p-6 shadow-sm hover:shadow-md transition-all duration-300 cursor-pointer group animate-[comeFromLeft_1s]'>
          <h2 className='text-base sm:text-lg md:text-[20px] font-medium text-[#172A39] group-hover:text-[#FC563C] transition'>
            {t('reservation.activeReservations')}
          </h2>
          <p className='text-[#6B7A8C] mt-2 text-sm sm:text-sm md:text-base'>
            {t('reservation.activeReservationsDesc')}
          </p>
        </Link>

        <Link to='/past-reservations' className='bg-white rounded-2xl p-4 sm:p-5 md:p-6 shadow-sm hover:shadow-md transition-all duration-300 cursor-pointer group animate-[comeFromRight_1s]'>
          <h2 className='text-base sm:text-lg md:text-[20px] font-medium text-[#172A39] group-hover:text-[#FC563C] transition'>
            {t('reservation.pastReservations')}
          </h2>
          <p className='text-[#6B7A8C] mt-2 text-sm sm:text-sm md:text-base'>
            {t('reservation.pastReservationsDesc')}
          </p>
        </Link>
      </div>

      <Link
        to='/create-reservation'
        onMouseEnter={handleEnter}
        onMouseLeave={handleLeave}
        className='fixed bottom-6 right-6 sm:bottom-8 sm:right-8 md:bottom-10 md:right-10 lg:bottom-10 lg:right-10 z-50'
      >
        <div className='
          bg-[#FC563C]
          hover:bg-[#172A39]
          text-white
          flex items-center justify-center
          cursor-pointer
          shadow-xl hover:shadow-2xl
          transition-all duration-500
          w-14 h-14 sm:w-16 sm:h-16 md:w-18 md:h-18 lg:w-20 lg:h-20
          lg:hover:w-72 lg:hover:h-16
          rounded-full lg:hover:rounded-6xl
          text-2xl sm:text-3xl lg:text-[30px] lg:hover:text-[20px]
          px-5
          backdrop-blur-md
        '>
          {text}
        </div>
      </Link>

    </div>
  )
}

export default Reservation