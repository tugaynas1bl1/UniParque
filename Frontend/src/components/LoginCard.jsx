import React, { useEffect, useState } from 'react'
import api from '../utils/axios.js'
import { useTokens } from '../stores/tokenStore'
import { useNavigate } from 'react-router-dom'

const LoginCard = () => {
  const navigate = useNavigate()
  const { setAccessToken, setRefreshToken } = useTokens()

  const [loginData, setLoginData] = useState({ email: '', password: '' })
  const [errors, setErrors] = useState({})
  const [translateLoginX, setTranslateLoginX] = useState(400)

  const handleInput = (key, value) => {
    setLoginData(prev => ({ ...prev, [key]: value }))
    setErrors(prev => ({ ...prev, [key]: '' }))
  }

  const validate = () => {
    let newErrors = {}

    if (!loginData.email.trim()) newErrors.email = "Email is required"
    if (!loginData.password) newErrors.password = "Password is required"

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleLogin = async () => {
    if (!validate()) return

    try {
      const { data } = await api.post('auth/login', loginData)

      setAccessToken(data.data.accessToken)
      setRefreshToken(data.data.refreshToken)

      if (data.data.isAdmin) {
        navigate('/admin/dashboard')
        document.body.style.overflow = "auto"
      } else {
        navigate('/')
      }
    } catch {
      setErrors({ general: "Invalid email or password" })
    }
  }

  useEffect(() => {
    setTimeout(() => setTranslateLoginX(0), 1300)
  }, [])

  return (
    <div
      className="lg:animate-[comeFromLeft_1s] animate-[comeFromBottom_1s] w-full max-w-md sm:max-w-lg md:max-w-xl p-4 sm:p-6 md:p-8 rounded-2xl"
    >
      <h1 className="text-black text-2xl sm:text-3xl font-bold text-center mb-6">
        Login
      </h1>

      <div className="flex flex-col gap-2 w-full">

        <input
          type="email"
          placeholder="Email"
          value={loginData.email}
          onChange={(e) => handleInput('email', e.target.value)}
          className={`px-4 py-3 rounded-xl border 
          ${errors.email ? 'border-red-500' : 'border-black/10'}
          bg-black/5 text-black outline-none focus:ring-2 focus:ring-[#FC563C]`}
        />
        {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}

        <input
          type="password"
          placeholder="Password"
          value={loginData.password}
          onChange={(e) => handleInput('password', e.target.value)}
          className={`px-4 py-3 rounded-xl border 
          ${errors.password ? 'border-red-500' : 'border-black/10'}
          bg-black/5 text-black outline-none focus:ring-2 focus:ring-[#FC563C]`}
        />
        {errors.password && <p className="text-red-500 text-sm">{errors.password}</p>}

        {errors.general && (
          <p className="text-red-500 text-sm text-center mt-2">
            {errors.general}
          </p>
        )}
      </div>

      <p
        onClick={() => navigate('/forgot-password')}
        className="cursor-pointer ml-1 text-black/65 hover:underline text-sm mt-4"
      >
        Forgot Password
      </p>

      <button
        onClick={handleLogin}
        className="mt-6 w-full py-3 rounded-2xl text-white font-bold transition-all duration-300 shadow-lg
        bg-gradient-to-r from-[#7a1500] to-[#FC563C]
        hover:shadow-[0_0_20px_rgba(252,86,60,0.5)]"
      >
        Login
      </button>
    </div>
  )
}

export default LoginCard