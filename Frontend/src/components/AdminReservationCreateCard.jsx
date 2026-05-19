import React, { useState, useEffect } from 'react';

const AdminReservationCreateCard = ({ setIsVisible, onReserve }) => {
  const [userId, setUserId] = useState('');
  const [carNumber, setCarNumber] = useState('');
  const [arrivalTime, setArrivalTime] = useState('');

  useEffect(() => {
    document.body.style.overflow = 'hidden';
    return () => { document.body.style.overflow = 'auto'; };
  }, []);

  const handleSubmit = () => {
    if (!userId || !carNumber || !arrivalTime) return;

    onReserve({
      userId,
      carNumber,
      estimatedArrivalTime: new Date(arrivalTime).toISOString()
    });
  };

  return (
    <div
      className="fixed inset-0 flex items-center justify-center bg-black/40 backdrop-blur-sm z-50"
      onClick={() => setIsVisible(false)}
    >
      <div
        className="w-[400px] bg-white rounded-2xl shadow-xl p-6 animate-fadeIn"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-xl font-semibold mb-4 text-gray-800">
          Create Reservation
        </h2>

        <input
          type="text"
          placeholder="User ID"
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          className="w-full mb-3 px-4 py-2 border rounded-lg text-black"
        />

        <input
          type="text"
          placeholder="Car Number"
          value={carNumber}
          onChange={(e) => setCarNumber(e.target.value)}
          className="w-full mb-3 px-4 py-2 border rounded-lg text-black"
        />

        <input
          type="datetime-local"
          value={arrivalTime}
          onChange={(e) => setArrivalTime(e.target.value)}
          className="w-full mb-6 px-4 py-2 border rounded-lg text-black"
        />

        <div className="flex justify-end gap-3">
          <button
            onClick={() => setIsVisible(false)}
            className="px-4 py-2 bg-gray-300 rounded-lg"
          >
            Cancel
          </button>

          <button
            onClick={handleSubmit}
            className="px-4 py-2 bg-orange-500 text-white rounded-lg"
          >
            Reserve
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminReservationCreateCard;