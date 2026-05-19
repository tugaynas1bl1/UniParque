import React, { useState } from "react";
import api from "../utils/axios";
import toast from "react-hot-toast";
import { useTranslation } from 'react-i18next';

const ChangePassword = () => {
  const { t } = useTranslation();

  const [form, setForm] = useState({
    oldPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };

  const validate = () => {
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/;

    if (!form.oldPassword || !form.newPassword || !form.confirmPassword) {
      toast.error(t('changePassword.fillAllFields'));
      return false;
    }

    if (form.newPassword.length < 8) {
      toast.error(t('changePassword.minLength'));
      return false;
    }

    if (!regex.test(form.newPassword)) {
      toast.error(t('changePassword.requirements'));
      return false;
    }

    if (form.newPassword !== form.confirmPassword) {
      toast.error(t('changePassword.passwordMismatch'));
      return false;
    }

    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validate()) return;

    try {
      setLoading(true);

      await api.put("auth/change-password", {
        oldPassword: form.oldPassword,
        newPassword: form.newPassword,
        confirmNewPassword: form.confirmPassword
      });

      toast.success(t('changePassword.success'));

      setForm({
        oldPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
      setInterval(() => {
        window.location.reload()
      }, 2000)
      
    } catch (err) {
      toast.error(
        err.response?.data?.message || t('changePassword.oldPasswordIncorrect')
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex absolute z-2 w-full justify-center items-center min-h-screen bg-gray-950 px-4">
      <form
        onSubmit={handleSubmit}
        className="w-full max-w-md bg-white p-6 rounded-2xl shadow-lg"
      >
        <h2 className="text-2xl font-semibold mb-6 text-center">
          {t('changePassword.title')}
        </h2>

        <input
          type="password"
          name="oldPassword"
          placeholder={t('changePassword.oldPassword')}
          value={form.oldPassword}
          onChange={handleChange}
          className="w-full mb-4 px-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-[#FC563C]"
        />

        <input
          type="password"
          name="newPassword"
          placeholder={t('changePassword.newPassword')}
          value={form.newPassword}
          onChange={handleChange}
          className="w-full mb-4 px-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-[#FC563C]"
        />

        <input
          type="password"
          name="confirmPassword"
          placeholder={t('changePassword.confirmPassword')}
          value={form.confirmPassword}
          onChange={handleChange}
          className="w-full mb-6 px-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-[#FC563C]"
        />

        <button
          type="submit"
          disabled={loading}
          className={`w-full py-3 rounded-lg text-white transition-all duration-300 
          ${
            loading
              ? "bg-gray-400 cursor-not-allowed"
              : "bg-gradient-to-r from-[#FC563C] to-[#ff8a70] hover:scale-105 hover:opacity-90"
          }`}
        >
          {loading ? t('changePassword.updating') : t('changePassword.updateButton')}
        </button>
      </form>
    </div>
  );
};

export default ChangePassword;