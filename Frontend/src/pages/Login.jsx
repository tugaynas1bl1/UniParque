import React, { useState, useEffect } from 'react'
import LoginCard from '../components/LoginCard.jsx'
import RegisterCard from '../components/RegisterCard.jsx'
import car from '../assets/car-black.png'
import { Link } from 'react-router-dom'

const Login = () => {
  const [isLogin, setIsLogin] = useState(
    () => localStorage.getItem('authPage') !== 'register'
  )

  useEffect(() => {
    document.body.style.overflowX = 'hidden'
  }, [])

  const switchToRegister = () => {
    localStorage.setItem('authPage', 'register')
    setIsLogin(false)
  }

  const switchToLogin = () => {
    localStorage.setItem('authPage', 'login')
    setIsLogin(true)
  }

  return (
    <div className={`${isLogin ? "justify-center lg:justify-start" : "justify-start lg:justify-center"} min-h-screen flex flex-col lg:flex-row bg-white`}>

      <div className="w-full lg:w-1/2 flex items-center justify-center px-4 sm:px-6 md:px-10 py-10">

        <div className="w-full max-w-md sm:max-w-lg md:max-w-xl">

          <div className={`transition-all duration-500 ${isLogin ? 'block' : 'hidden'}`}>
            <LoginCard />
          </div>

          <div className={`flex mt-20 lg:mt-0 transition-all duration-500 ${!isLogin ? 'translate-y-50 block lg:translate-y-0 lg:absolute  lg:top-50 lg:right-20' : 'hidden'}`}>
            <RegisterCard onGoLogin={switchToLogin} />
          </div>

        </div>
      </div>

      <div className={`${!isLogin ? "lg:animate-[comeFromLeft_1s] -translate-y-10 lg:translate-y-0 lg:left-0"
        : "lg:animate-[comeFromRight_1s] translate-y-100 lg:translate-y-0 lg:right-0"} w-full lg:w-1/2 flex 
        flex-col items-center justify-center text-center absolute bg-gradient-to-br lg:h-screen 
        animate-[comeFromTop_1s]  from-[#7a1500] to-[#103465] gap-10
      text-white px-6 sm:px-10 py-12 transition-all duration-500`}>


        <div className="z-10 max-w-md flex flex-col items-center gap-6">
          <Link to='/' className='self-start mx-auto lg:mx-15 lg:absolute top-10 left-0
           cursor-pointer hover:border-double border-hidden 
           border-1 p-1 hover:p-3 rounded-3xl outline-none 
           transition-all duration-300'>⇦ Go Home</Link>

          <h1 className="text-3xl sm:text-4xl md:text-5xl font-bold">
            {isLogin ? 'Hello, Welcome' : 'Welcome Back!'}
          </h1>

          <p className="text-white/80 text-sm sm:text-base">
            {isLogin
              ? "Don't have an account?"
              : 'Already have an account?'}
          </p>

          <button
            onClick={isLogin ? switchToRegister : switchToLogin}
            className="border border-white px-8 py-3 rounded-full
            hover:bg-white hover:text-black
            transition-all duration-300"
          >
            {isLogin ? 'Register' : 'Login'}
          </button>

        </div>
      </div>
    </div>
  )
}

export default Login