import React, { useState, useRef } from 'react';
import toast from 'react-hot-toast';
import api from '../utils/axios';
import Loader from '../components/Loader';

const ForgotPassword = () => {
  const [email, setEmail] = useState('');
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [newPassword, setNewPassword] = useState('');
  const [confirmNewPassword, setConfirmNewPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const [errors, setErrors] = useState({});
  const [strength, setStrength] = useState('');

  const inputRefs = useRef([]);

  // 🔥 Password strength
  const calculateStrength = (password) => {
    let score = 0;
    if (password.length >= 8) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[a-z]/.test(password)) score++;
    if (/[0-9]/.test(password)) score++;

    if (score <= 2) return 'Weak';
    if (score === 3) return 'Medium';
    return 'Strong';
  };

  // 🔥 Validation (real-time)
  const validate = () => {
    let newErrors = {};

    if (!email.trim()) newErrors.email = "Email is required";

    if (code.some(c => c === ''))
      newErrors.code = "Enter full 6 digit code";

    if (!newPassword)
      newErrors.newPassword = "Password is required";
    else {
      if (newPassword.length < 8)
        newErrors.newPassword = "Min 8 characters";
      else if (!/[A-Z]/.test(newPassword))
        newErrors.newPassword = "Need uppercase letter";
      else if (!/[a-z]/.test(newPassword))
        newErrors.newPassword = "Need lowercase letter";
      else if (!/[0-9]/.test(newPassword))
        newErrors.newPassword = "Need a digit";
    }

    if (!confirmNewPassword)
      newErrors.confirmNewPassword = "Confirm password";
    else if (confirmNewPassword !== newPassword)
      newErrors.confirmNewPassword = "Passwords don't match";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // 🔥 Send code
  const sendCode = async () => {
    if (!email.trim()) {
      setErrors(prev => ({ ...prev, email: "Email is required" }));
      return;
    }

    try {
      setIsLoading(true);
      const { status } = await api.post('auth/send-verification-code', { email });

      if (status === 200) {
        toast.success('Verification code sent!');
      }
    } catch (err) {
      toast.error('Failed to send code.');
    } finally {
      setIsLoading(false);
    }
  };

  // 🔥 Code input
  const handleCodeChange = (e, index) => {
    const val = e.target.value;

    if (/^[0-9]?$/.test(val)) {
      const newCode = [...code];
      newCode[index] = val;
      setCode(newCode);

      if (val && index < 5) {
        inputRefs.current[index + 1].focus();
      }

      if (!val && index > 0) {
        inputRefs.current[index - 1].focus();
      }
    }
  };

  // 🔥 Password change
  const changePassword = async () => {
    if (!validate()) return;

    try {
      await api.put('auth/change-forgotten-password', {
        code: parseInt(code.join('')),
        email,
        newPassword,
        confirmNewPassword,
      });

      toast.success('Password changed successfully!');

      setCode(['', '', '', '', '', '']);
      setNewPassword('');
      setConfirmNewPassword('');
      setEmail('');
      setErrors({});
      setStrength('');
    } catch (err) {
      toast.error('Failed to change password');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white w-[400px] p-8 rounded-2xl shadow-xl">

        <h2 className="text-2xl font-semibold text-center mb-6">
          Forgot Password
        </h2>

        {/* EMAIL */}
        <input
          type="email"
          placeholder="Enter your email"
          value={email}
          onChange={e => {
            setEmail(e.target.value);
            setErrors(prev => ({ ...prev, email: "" }));
          }}
          className={`w-full p-3 border rounded-lg mb-1 focus:outline-none focus:ring-2 
          ${errors.email ? "border-red-500 ring-red-300" : "focus:ring-blue-500"}`}
        />
        {errors.email && <p className="text-red-500 text-sm mb-3">{errors.email}</p>}

        <div
          onClick={sendCode}
          className={`w-full flex justify-center items-center py-3 rounded-lg mb-6 text-white transition
          ${email ? "bg-orange-500 hover:bg-orange-600 cursor-pointer" : "bg-gray-400 cursor-not-allowed"}`}
        >
          Send Code
          {isLoading && <span className="ml-2"><Loader size={18} /></span>}
        </div>

        <div className="flex justify-between mb-2">
          {code.map((num, idx) => (
            <input
              key={idx}
              type="text"
              maxLength="1"
              value={num}
              onChange={e => handleCodeChange(e, idx)}
              ref={el => inputRefs.current[idx] = el}
              className="w-12 h-14 text-xl text-center border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
          ))}
        </div>
        {errors.code && <p className="text-red-500 text-sm mb-4">{errors.code}</p>}

        <input
          type="password"
          placeholder="New Password"
          value={newPassword}
          onChange={e => {
            setNewPassword(e.target.value);
            setStrength(calculateStrength(e.target.value));
          }}
          className={`w-full p-3 border rounded-lg mb-1 focus:outline-none
          ${errors.newPassword ? "border-red-500" : ""}`}
        />

        {newPassword && (
          <p className={`text-sm mb-2 ${
            strength === 'Weak' ? 'text-red-500' :
            strength === 'Medium' ? 'text-yellow-500' :
            'text-green-500'
          }`}>
            Strength: {strength}
          </p>
        )}

        {errors.newPassword && <p className="text-red-500 text-sm mb-3">{errors.newPassword}</p>}

        <input
          type="password"
          placeholder="Confirm Password"
          value={confirmNewPassword}
          onChange={e => setConfirmNewPassword(e.target.value)}
          className={`w-full p-3 border rounded-lg mb-1 ${errors.confirmNewPassword ? "border-red-500" : ""}`}
        />
        {errors.confirmNewPassword && <p className="text-red-500 text-sm mb-4">{errors.confirmNewPassword}</p>}

        {/* BUTTON */}
        <button
          onClick={changePassword}
          className="w-full bg-green-500 hover:bg-green-600 text-white py-3 rounded-lg transition"
        >
          Change Password
        </button>

      </div>
    </div>
  );
};

export default ForgotPassword;