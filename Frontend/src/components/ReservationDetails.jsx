import React, { useEffect, useState } from 'react';
import api from '../utils/axios';
import toast from 'react-hot-toast';
import warning from '../assets/warning.png';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

const ReservationDetails = ({ bookedData, onClose }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTime, setSelectedTime] = useState('');
  const [cardNumber, setCardNumber] = useState('');
  const [carNumber, setCarNumber] = useState('');
  const [loading, setLoading] = useState(false);
  const [expiry, setExpiry] = useState("");
  const [cvv, setCvv] = useState("");
  const [totalPrice, setTotalPrice] = useState(0.0);
  const [isCardPayHidden, setIsCardPayHidden] = useState(true);
  const [isBalancePayHidden, setIsBalancePayHidden] = useState(true);
  const [isDetailInputsHidden, setIsDetailInputsHidden] = useState(false);
  const [isPayOptionsHidden, setIsPayOptionsHidden] = useState(true);
  const [userBalance, setUserBalance] = useState(0.0);

  const date = new Date(selectedDate);
  const day = date.getDate().toString().padStart(2, "0");
  const month = (date.getMonth() + 1).toString().padStart(2, "0");
  const year = date.getFullYear();
  const fullDate = `${day}.${month}.${year}  ${selectedTime}`;

  if (!bookedData?.place) return null;

  const handleExpiryChange = (e) => {
    let value = e.target.value.replace(/\D/g, "");
    if (value.length > 4) value = value.slice(0, 4);
    if (value.length >= 3) value = value.slice(0, 2) + "/" + value.slice(2);
    setExpiry(value);
  };

  const calculatePrice = async (date) => {
    try {
      const localDateTime = new Date(date);
      const payload = { estimatedArrivalTime: localDateTime.toISOString() };
      const { data } = await api.post('reservations/calculate-price', payload);
      setTotalPrice(data.data);
    } catch (error) {
      console.error(error);
    }
  };

  const getUserBalance = async () => {
    try {
      const { data } = await api.get('auth/my-balance');
      if (data.success) setUserBalance(data.data);
    } catch (error) {
      console.error(error);
    }
  };

  const handlePay = async () => {
    if (!cardNumber || !expiry || !cvv) {
      toast(t('reservationDetails.fillAllPayFields'), {
        icon: <img className='w-8 h-8' src={warning} />,
        className: 'text-center font-bold'
      });
      return;
    }

    const cleanNumber = cardNumber.replace(/\s/g, "");
    const localDateTime = new Date(`${selectedDate}T${selectedTime}:00`);

    if (!/^\d{16}$/.test(cleanNumber)) {
      toast.error(t('reservationDetails.invalidCard'));
      return;
    }

    if (localDateTime.getTime() <= Date.now()) {
      toast.error(t('reservationDetails.invalidDate'));
      return;
    }

    const utcDateTime = localDateTime.toISOString();
    const payload = {
      cardNumber: cleanNumber,
      reservation: {
        placeId: bookedData.place.id,
        estimatedArrivalTime: utcDateTime,
        carNumber: carNumber
      }
    };

    try {
      const { data } = await api.post('reservations/create', payload);
      if (data.success) {
        toast.success(t('reservationDetails.successReservation'));
        setTimeout(() => {
          window.location.reload();
        }, 2000)
      }
    } catch (error) {
      toast.error("Something went wrong!");
      console.error(error);
    }
  };

  const handleConfirm = async () => {
    if (!selectedDate || !selectedTime || !carNumber) {
      toast(t('reservationDetails.fillAllFields'), {
        icon: <img className='w-8 h-8' src={warning} />,
        className: 'text-center font-bold'
      });
      return;
    }

    const localDateTime = new Date(`${selectedDate}T${selectedTime}`);
    if (localDateTime.getTime() <= Date.now()) {
      toast.error(t('reservationDetails.invalidDate'));
      return;
    }

    setIsDetailInputsHidden(true);
    calculatePrice(localDateTime.getTime());
    setIsPayOptionsHidden(false);
  };

  const onReturn = () => {
    setIsCardPayHidden(true);
    setIsDetailInputsHidden(false);
    setCardNumber("");
    setExpiry("");
    setCvv("");
    setIsPayOptionsHidden(true);
    setIsBalancePayHidden(true);
  };

  const onClickPayOffBalance = () => {
    setIsBalancePayHidden(false);
    setIsPayOptionsHidden(true);
  };

  const onClickPayByCard = () => {
    setIsCardPayHidden(false);
    setIsPayOptionsHidden(true);
  };

  const onPayClickedBalance = async (amount) => {
    if (userBalance < amount) {
      toast.error(t('reservationDetails.insufficientFunds'));
      return;
    }

    const localDateTime = new Date(`${selectedDate}T${selectedTime}`);
    if (localDateTime.getTime() <= Date.now()) {
      toast.error(t('reservationDetails.invalidDate'));
      return;
    }

    const utcDateTime = localDateTime.toISOString();
    const payload = {
      placeId: bookedData.place.id,
      estimatedArrivalTime: utcDateTime,
      carNumber: carNumber
    };

    try {
      const { data } = await api.post('reservations/create-by-balance', payload);
      if (data.success) {
        toast.success(t('reservationDetails.successReservation'));

        setTimeout(() => {
          window.location.reload();
        }, 2000)
      }
    } catch (error) {
      toast.error("Something went wrong!");
      console.error(error);
    }
  };

  useEffect(() => {
    getUserBalance();
  }, []);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm px-3 sm:px-4 md:px-6">
      <div className="bg-[#0B0F14] border border-[#FC563C] rounded-2xl w-full sm:w-[90%] md:w-[420px] lg:w-96 p-4 sm:p-5 md:p-6 relative shadow-2xl max-h-[90vh] overflow-y-auto">

        <button onClick={onReturn} className="absolute top-4 sm:top-5 md:top-6 left-4 text-gray-400 hover:text-[#FC563C] text-lg sm:text-xl">
          ⇦
        </button>

        <button onClick={onClose} className="absolute top-4 sm:top-5 md:top-6 right-4 text-gray-400 hover:text-[#FC563C] text-lg sm:text-xl">
          ×
        </button>

        <h2 className="text-xl sm:text-2xl font-bold text-[#FC563C] mb-3 sm:mb-4 text-center pt-1">
          {t('reservationDetails.title')}
        </h2>

        <div className="text-gray-300 space-y-1.5 sm:space-y-2 mb-3 sm:mb-4 text-sm sm:text-sm md:text-base">
          <p><span className="font-semibold">{t('reservationDetails.branch')}</span> {bookedData.branch}</p>
          <p><span className="font-semibold">{t('reservationDetails.section')}</span> {bookedData.section}</p>
          <p><span className="font-semibold">{t('reservationDetails.subSection')}</span> {bookedData.subSection}</p>
          <p><span className="font-semibold">{t('reservationDetails.placeId')}</span> {bookedData.place?.id}</p>
          {isDetailInputsHidden && <p><span className="font-semibold">{t('reservationDetails.carNumber')}</span> {carNumber}</p>}
          {isDetailInputsHidden && <p><span className="font-semibold">{t('reservationDetails.arrivalDate')}</span> {fullDate}</p>}
        </div>

        <div className={`${isDetailInputsHidden ? "hidden" : ""} flex flex-col`}>
          <input
            type="text"
            placeholder="10-AA-001"
            value={carNumber}
            onChange={(e) => setCarNumber(e.target.value)}
            className="w-full mb-6 sm:mb-8 md:mb-10 p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
          />
          <div className="mb-3 sm:mb-4 flex gap-2">
            <input
              type="date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              className="w-1/2 p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
            />
            <input
              type="time"
              value={selectedTime}
              onChange={(e) => setSelectedTime(e.target.value)}
              className="w-1/2 p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
            />
          </div>
        </div>

        <button
          onClick={handleConfirm}
          disabled={loading}
          className={`${isDetailInputsHidden ? "hidden" : ""} w-full bg-[#FC563C] hover:bg-white hover:text-[#FC563C] text-white py-2.5 sm:py-3 rounded-xl text-sm sm:text-base font-semibold transition disabled:opacity-50`}
        >
          {loading ? t('reservationDetails.loading') : t('reservationDetails.confirmReservation')}
        </button>

        {isDetailInputsHidden && (
          <p className='text-[#FC563C] text-lg sm:text-[20px] font-bold mb-4 sm:mb-5 mt-4 sm:mt-6 text-center'>
            <span className="font-semibold text-sm sm:text-[16px] text-white">{t('reservationDetails.totalPrice')}</span> ₼{totalPrice}
          </p>
        )}
        {isDetailInputsHidden && isCardPayHidden && isPayOptionsHidden && (
          <p className='text-[#FC563C] text-lg sm:text-[20px] font-bold mb-4 sm:mb-5 mt-4 sm:mt-6 text-center'>
            <span className="font-semibold text-sm sm:text-[16px] text-white">{t('reservationDetails.accountBalance')}</span> ₼{userBalance}
          </p>
        )}

        <div className={`${isPayOptionsHidden ? "hidden" : ""} flex gap-3 sm:gap-5`}>
          <button
            onClick={onClickPayByCard}
            disabled={loading}
            className="w-full bg-[#FC563C] hover:bg-white hover:text-[#FC563C] text-white py-2.5 sm:py-3 rounded-xl text-sm sm:text-base font-semibold"
          >
            {t('reservationDetails.payByCard')}
          </button>
          <button
            onClick={onClickPayOffBalance}
            disabled={loading}
            className="w-full bg-[#FC563C] hover:bg-white hover:text-[#FC563C] text-white py-2.5 sm:py-3 rounded-xl text-sm sm:text-base font-semibold"
          >
            {t('reservationDetails.payBalance')}
          </button>
        </div>

        <div className={`${isCardPayHidden ? "hidden" : ""} mb-3 sm:mb-4 space-y-2`}>
          <input
            type="text"
            placeholder={t('reservationDetails.cardNumber')}
            value={cardNumber}
            onChange={(e) => setCardNumber(e.target.value)}
            className="w-full p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
          />
          <div className='flex gap-3 sm:gap-5'>
            <input
              type="text"
              placeholder="MM/YY"
              value={expiry}
              onChange={handleExpiryChange}
              className="w-full p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
            />
            <input
              type="text"
              placeholder="CVV"
              value={cvv}
              onChange={(e) => setCvv(e.target.value)}
              className="w-full p-2 rounded-lg bg-[#11161D] border border-white/20 text-white text-sm sm:text-base"
            />
          </div>
          <button
            onClick={handlePay}
            disabled={loading}
            className="w-full bg-[#FC563C] hover:bg-white hover:text-[#FC563C] text-white py-2.5 sm:py-3 rounded-xl text-sm sm:text-base font-semibold"
          >
            {t('reservationDetails.pay')}
          </button>
        </div>

        <button
          onClick={() => onPayClickedBalance(totalPrice)}
          disabled={loading}
          className={`${isBalancePayHidden ? "hidden" : ""} w-full bg-[#FC563C] hover:bg-white hover:text-[#FC563C] text-white py-2.5 sm:py-3 rounded-xl text-sm sm:text-base font-semibold`}
        >
          {t('reservationDetails.pay')}
        </button>

      </div>
    </div>
  );
};

export default ReservationDetails;