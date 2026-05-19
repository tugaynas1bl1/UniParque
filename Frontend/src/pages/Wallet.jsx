import React, { useEffect, useState } from "react";
import api from "../utils/axios";
import toast from "react-hot-toast";

const Wallet = () => {
  const [balance, setBalance] = useState(0);
  const [activeTab, setActiveTab] = useState("deposit");

  const [form, setForm] = useState({
    cardNumber: "",
    cvv: "",
    expiry: "",
    amount: ""
  });

  // Balansı çəkmək
  const getBalance = async () => {
    try {
      const { data } = await api.get("auth/my-balance");
      if (data.success) setBalance(data.data);
    } catch (err) {
      console.error(err);
      toast.error("Cannot fetch balance");
    }
  };

  useEffect(() => {
    getBalance();
  }, []);

  const handleChange = (e) => {
    let { name, value } = e.target;

    if (name === "cardNumber") {
      value = value.replace(/\D/g, "").slice(0, 16);
    }

    if (name === "cvv") {
      value = value.replace(/\D/g, "").slice(0, 3);
    }

    if (name === "expiry") {
      value = value.replace(/\D/g, "");
      if (value.length > 4) value = value.slice(0, 4);
      if (value.length >= 3) value = value.slice(0, 2) + "/" + value.slice(2);
    }

    if (name === "amount") {
      value = value.replace(/[^0-9.]/g, "");
    }

    setForm({ ...form, [name]: value });
  };

  const validate = () => {
    const { cardNumber, cvv, expiry, amount } = form;

    if (!cardNumber || !cvv || !expiry || !amount) {
      toast.error("Fill all fields");
      return false;
    }

    if (cardNumber.length !== 16) {
      toast.error("Card number must be 16 digits");
      return false;
    }

    if (cvv.length !== 3) {
      toast.error("CVV must be 3 digits");
      return false;
    }

    if (!/^\d{2}\/\d{2}$/.test(expiry)) {
      toast.error("Expiry must be in MM/YY format");
      return false;
    }

    if (parseFloat(amount) > balance && activeTab === "withdraw") {
      toast.error("Insufficient funds");
      return false;
    }

    if (parseFloat(amount) <= 0) {
      toast.error("Invalid amount");
      return false;
    }

    return true;
  };

  const handleSubmit = async () => {
    if (!validate()) return;

    try {
      const { data } = await api.patch("auth/adjust-balance", {
        amount: parseFloat(form.amount),
        cardNumber: form.cardNumber,
        type: activeTab
      });

      if (data.data) {
        toast.success(
          activeTab === "deposit" ? "Balance increased!" : "Money withdrawn!"
        );

        getBalance();

        setForm({
          cardNumber: "",
          cvv: "",
          expiry: "",
          amount: ""
        });

        setTimeout(() => {
          window.location.reload();
        }, 2000);
      }
    } catch (err) {
      toast.error("Operation failed");
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-start sm:items-center py-10 sm:py-0 bg-gray-950">
      <div className="bg-white p-6 rounded-2xl w-11/12 sm:w-96 shadow-lg mt-20">
        <h2 className="text-2xl font-bold text-center mb-4">Wallet</h2>

        <p className="text-center mb-6 text-[#FC563C] font-semibold text-lg">
          Balance: ₼{balance}
        </p>

        <div className="flex mb-4 gap-2 sm:gap-0">
          <button
            onClick={() => setActiveTab("deposit")}
            className={`flex-1 p-2 cursor-pointer rounded ${
              activeTab === "deposit"
                ? "bg-[#FC563C] text-white"
                : "bg-gray-200 text-gray-700"
            }`}
          >
            Deposit Balance
          </button>
          <button
            onClick={() => setActiveTab("withdraw")}
            className={`flex-1 p-2 cursor-pointer rounded ${
              activeTab === "withdraw"
                ? "bg-[#FC563C] text-white"
                : "bg-gray-200 text-gray-700"
            }`}
          >
            Withdraw
          </button>
        </div>

        <div className="flex flex-col sm:flex-col gap-3">
          <input
            name="cardNumber"
            placeholder="Card Number"
            value={form.cardNumber}
            onChange={handleChange}
            className="w-full px-3 py-2 border rounded focus:outline-none"
          />
          <input
            name="expiry"
            placeholder="MM/YY"
            value={form.expiry}
            onChange={handleChange}
            className="w-full px-3 py-2 border rounded focus:outline-none"
          />
          <input
            name="cvv"
            placeholder="CVV"
            value={form.cvv}
            onChange={handleChange}
            className="w-full px-3 py-2 border rounded focus:outline-none"
          />
          <input
            name="amount"
            placeholder="Amount"
            value={form.amount}
            onChange={handleChange}
            className="w-full px-3 py-2 border rounded focus:outline-none"
          />
        </div>

        <button
          onClick={handleSubmit}
          className="w-full mt-4 cursor-pointer bg-[#FC563C] text-white py-2 rounded hover:opacity-90 transition"
        >
          {activeTab === "deposit" ? "Add Balance" : "Withdraw"}
        </button>
      </div>
    </div>
  );
};

export default Wallet;