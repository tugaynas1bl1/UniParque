import React from 'react'

const VerificationNotification = ({
  message,
  setShowVerificationVisible,
  onConfirm
}) => {

  const handleCancel = () => {
    setShowVerificationVisible(false)
  }

  const handleOkay = () => {
    if (onConfirm)
      onConfirm()

    setShowVerificationVisible(false)
  }

  return (
    <div className="fixed inset-0 z-50">

      {/* Overlay */}
      <div className="absolute inset-0 bg-black/20"></div>

      {/* Modal */}
      <div
        className="
          absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2
          w-[90%] sm:w-[75%] md:w-[500px] lg:w-140
          min-h-[220px] sm:min-h-[250px] lg:h-70
          bg-white rounded-xl shadow-lg flex flex-col
          px-4 sm:px-6
        "
      >

        <p
          className="
            absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2
            font-semibold text-center
            text-[16px] sm:text-[18px] lg:text-[20px]
            w-[85%]
            break-words
          "
        >
          {message}
        </p>

        <div
  className="
    flex
    w-full px-5 lg:w-[90%]
    gap-6 sm:gap-6 lg:gap-10
    absolute
    bottom-5 left-1/2 -translate-x-1/2
    sm:justify-end
  "
>

  <button
    onClick={handleOkay}
    className="
      bg-blue-500 w-full lg:w-fit hover:bg-blue-400 text-white
      px-4 py-2
      rounded-lg font-semibold
      cursor-pointer hover:scale-105
      transition-all duration-300
    "
  >
    Okay
  </button>

  <button
    onClick={handleCancel}
    className="
      bg-red-600 w-full lg:w-fit hover:bg-red-900 text-white
      px-4 py-2
      rounded-lg font-semibold
      cursor-pointer hover:scale-105
      transition-all duration-300
    "
  >
    Cancel
  </button>

</div>

      </div>

    </div>
  )
}

export default VerificationNotification