import React, { useState } from 'react'
import api from '../utils/axios.js'
import { useTokens } from '../stores/tokenStore'
import { useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'

const RegisterCard = ({ onGoLogin }) => {
  const navigate = useNavigate()
  const { setAccessToken, setRefreshToken } = useTokens()

  const [doesUserExist, setDoesUserExist] = useState(false)
  const [loading, setLoading] = useState(false)

  const [info, setInfo] = useState({
    firstName: '', lastName: '', email: '', password: '', confirmedPassword: ''
  })

  const [errors, setErrors] = useState({})
  const [strength, setStrength] = useState('')

  const handleInput = (key, value) => {
    setInfo(prev => ({ ...prev, [key]: value }))
    setErrors(prev => ({ ...prev, [key]: '' }))

    if (key === 'password') {
      setStrength(calculateStrength(value))
    }
  }

  const calculateStrength = (password) => {
    let score = 0
    if (password.length >= 8) score++
    if (/[A-Z]/.test(password)) score++
    if (/[a-z]/.test(password)) score++
    if (/[0-9]/.test(password)) score++

    if (score <= 2) return 'Weak'
    if (score === 3) return 'Medium'
    return 'Strong'
  }

  const validate = () => {
    let newErrors = {}

    if (!info.firstName.trim()) newErrors.firstName = "Required"
    if (!info.lastName.trim()) newErrors.lastName = "Required"
    if (!info.email.trim()) newErrors.email = "Required"

    if (!info.password) newErrors.password = "Required"
    else {
      if (info.password.length < 8) newErrors.password = "Min 8 chars"
      else if (!/[A-Z]/.test(info.password)) newErrors.password = "Need uppercase"
      else if (!/[a-z]/.test(info.password)) newErrors.password = "Need lowercase"
      else if (!/[0-9]/.test(info.password)) newErrors.password = "Need digit"
    }

    if (info.confirmedPassword !== info.password)
      newErrors.confirmedPassword = "Passwords do not match"

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleRegister = async () => {
    if (!validate()) return

    try {
      setLoading(true)

      const { data } = await api.post('auth/register', {
        FirstName: info.firstName,
        LastName: info.lastName,
        Email: info.email,
        Password: info.password,
        ConfirmedPassword: info.confirmedPassword
      })

      if (data.success) {
        toast.success("Registered successfully!")
        onGoLogin()
      }

    } catch {
      toast.error("Registration failed!")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="animate-[comeFromTop_1s] lg:animate-[comeFromRight_1s] w-full max-w-md sm:max-w-lg md:max-w-xl p-4 sm:p-6 md:p-8 rounded-2xl">

      <h1 className="text-black text-2xl sm:text-3xl font-bold text-center mb-6">
        Create Account
      </h1>

      {/* NAME GRID */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
        <div>
          <input
            value={info.firstName}
            placeholder="First Name"
            onChange={(e) => handleInput('firstName', e.target.value)}
            className={`px-4 py-3 rounded-xl border w-full 
            ${errors.firstName ? 'border-red-500' : 'border-black/10'}`}
          />
          {errors.firstName && <p className="text-red-500 text-sm">{errors.firstName}</p>}
        </div>

        <div>
          <input
            value={info.lastName}
            placeholder="Last Name"
            onChange={(e) => handleInput('lastName', e.target.value)}
            className={`px-4 py-3 rounded-xl border w-full 
            ${errors.lastName ? 'border-red-500' : 'border-black/10'}`}
          />
          {errors.lastName && <p className="text-red-500 text-sm">{errors.lastName}</p>}
        </div>
      </div>

      <div className="flex flex-col gap-2 mt-4">

        <input
          value={info.email}
          placeholder="Email"
          onChange={(e) => handleInput('email', e.target.value)}
          className={`px-4 py-3 rounded-xl border 
          ${errors.email ? 'border-red-500' : 'border-black/10'}`}
        />
        {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}

        <input
          type="password"
          value={info.password}
          placeholder="Password"
          onChange={(e) => handleInput('password', e.target.value)}
          className={`px-4 py-3 rounded-xl border 
          ${errors.password ? 'border-red-500' : 'border-black/10'}`}
        />

        {info.password && (
          <p className={`text-sm ${
            strength === 'Weak' ? 'text-red-500' :
            strength === 'Medium' ? 'text-yellow-500' :
            'text-green-500'
          }`}>
            Strength: {strength}
          </p>
        )}

        <input
          type="password"
          value={info.confirmedPassword}
          placeholder="Confirm Password"
          onChange={(e) => handleInput('confirmedPassword', e.target.value)}
          className={`px-4 py-3 rounded-xl border 
          ${errors.confirmedPassword ? 'border-red-500' : 'border-black/10'}`}
        />

        <button
          onClick={handleRegister}
          disabled={loading}
          className={`mt-2 py-3 rounded-xl text-white font-bold transition-all duration-300
          ${loading 
            ? 'bg-gray-400 cursor-not-allowed' 
            : 'bg-[#172A39] hover:bg-[#406b8b] hover:scale-105'
          }`}
        >
          {loading ? "Registering..." : "Register"}
        </button>

      </div>
    </div>
  )
}

export default RegisterCard