import React, { useEffect, useState } from 'react'
import logo from '../assets/Logo.png'
import { Link, useNavigate } from 'react-router-dom'
import { useTokens } from '../stores/tokenStore'
import api from '../utils/axios'
import LanguageSelector from './LanguageSelector'
import { useTranslation } from 'react-i18next'

const Header = ({ activeMenu }) => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const { accessToken, clearTokens } = useTokens()
  const isLoggedIn = Boolean(accessToken)
  const [profilePhoto, setProfilePhoto] = useState("")
  const [userBalance, setUserBalance] = useState(0.0)
  const [menuOpen, setMenuOpen] = useState(false)

  const getUserPhoto = async () => {
    if (!isLoggedIn) return
    try {
      const { data, statusText } = await api.get('auth/profile-photo', {
        headers: { Authorization: `Bearer ${accessToken}` },
      })
      if (statusText === 'OK') setProfilePhoto(data.data.url)
    } catch (error) {
      if (error.response?.status === 401) clearTokens()
      console.error(error)
    }
  }

  const getUserBalance = async () => {
    try {
      const { data } = await api.get('auth/my-balance')
      if (data.success) setUserBalance(data.data)
    } catch (error) {
      console.error(error)
    }
  }

  const checkAccess = async () => {
    if (accessToken) navigate('/reservation')
    else {
      alert(t('header.signInAlert'))
      localStorage.setItem('authPage', 'login')
      navigate('/login')
    }
  }

  useEffect(() => {
    if (!accessToken) {
      setProfilePhoto("")
      return
    }
    getUserPhoto()
    getUserBalance()
  }, [accessToken])

  return (
    <>
      <div className={`z-20 w-full text-[12px] lg:text-[16px] h-14 top-0 flex items-center backdrop-blur-md bg-black/10 justify-around fixed py-3`}>
        
        <div className='flex justify-center items-center gap-0 lg:gap-2'>
          <img className='w-10 h-10 lg:w-13 lg:h-13 xl:w-15 xl:h-15 img-outline' src={logo} alt="" />
          <p className='font-bold text[16px] text-[#29465d] lg:text-[17px] xl:text-[20px]'>
            <span className='text-[#FC563C]'>Uni</span>Parque
          </p>
        </div>

        {activeMenu !== "Login" && activeMenu !== "Register" && (
          <div className='hidden lg:flex items-center'>
            <div className='flex list-none gap-10 lg:text-[14px] lg:gap-5 xl:gap-10 xl:text-lg font-semibold text-shadow-amber-950'>
              <Link
                to='/'
                className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "Home" ? "text-[#FC563C]" : "text-[#6E7575] hover:text-[#FC563C]"}`}
              >
                {t('header.home')}
                <span className={`absolute left-0 -bottom-1 h-1 w-0 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Home" ? "w-full" : "hover:w-full"}`}></span>
              </Link>

              <button
                onClick={checkAccess}
                className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "Reservation" ? "text-[#FC563C]" : "text-[#6E7575] hover:text-[#FC563C]"}`}
              >
                {t('header.reservations')}
                <span className={`absolute left-0 -bottom-1 h-1 w-0 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Reservation" ? "w-full" : "hover:w-full"}`}></span>
              </button>

              <Link
                to='/about'
                className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "About" ? "text-[#FC563C]" : "text-[#6E7575] hover:text-[#FC563C]"}`}
              >
                {t('header.aboutUs')}
                <span className={`absolute left-0 -bottom-1 h-1 w-0 bg-[#FC563C] transition-all duration-500 ${activeMenu === "About" ? "w-full" : "hover:w-full"}`}></span>
              </Link>

              <Link to='/contact' className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "Contact" ? "text-[#FC563C]" : "text-[#6E7575] hover:text-[#FC563C]"}`}>
                {t('header.contact')}
                <span className={`absolute left-0 -bottom-1 h-1 w-0 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Contact" ? "w-full" : "hover:w-full"}`}></span>
              </Link>
            </div>
          </div>
        )}

        <div className='hidden lg:block -mt-5 -ml-40'>
          <LanguageSelector />
        </div>

        {!accessToken && (
          <button
            onClick={() => navigate('/login')}
            className="hidden lg:block bg-gradient-to-r from-[#FC563C] to-[#FF8A70] text-white px-6 py-2 rounded-lg font-semibold cursor-pointer transition-all duration-300 hover:scale-105"
          >
            {t('header.getStarted')}
          </button>
        )}

        {accessToken && (
          <div className='hidden lg:flex gap-3 items-center'>
            <p className='font-bold text-[#FC563C] whitespace-break-spaces lg:text-[14px] xl:text-[16px]'>
              {t('header.balance')}: <span className='lg:text-[16px] xl:text-[18px]'> ₼{userBalance}</span>
            </p>
            <div onClick={() => navigate('/my-wallet')} className='bg-[#172A39] text-white rounded-md w-5 h-5 cursor-pointer border-[2px] hover:bg-[#FC563C] pb-3.5 pt-2.5 hover:border-[#172A39] border-[#FC563C] flex justify-center items-center p-3 transition-all duration-300'>+</div>
          </div>
        )}

        {isLoggedIn && (
          <Link to='/profile' className='hidden lg:flex bg-[#15232e] hover:bg-[#FC563C] w-12 h-12 rounded-full justify-center items-center transition-all duration-500'>
            {profilePhoto ? (
              <img className='w-11 h-11 rounded-full object-cover' src={profilePhoto} alt="Profile photo"/>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="8" r="5"/><path d="M20 21a8 8 0 0 0-16 0"/></svg>
            )}
          </Link>
        )}

        <div className='flex lg:hidden items-center gap-3'>

        
          <LanguageSelector />

          {isLoggedIn && (
            <Link to='/profile' className='bg-[#15232e] hover:bg-[#FC563C] w-9 h-9 rounded-full flex justify-center items-center transition-all duration-500'>
              {profilePhoto ? (
                <img className='w-8 h-8 rounded-full object-cover' src={profilePhoto} alt="Profile photo"/>
              ) : (
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#ffffff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="8" r="5"/><path d="M20 21a8 8 0 0 0-16 0"/></svg>
              )}
            </Link>
          )}

          {activeMenu !== "Login" && activeMenu !== "Register" && (
            <button
              onClick={() => setMenuOpen(!menuOpen)}
              className='flex flex-col justify-center items-center gap-1.5 w-9 h-9 cursor-pointer'
            >
              <span className={`block w-6 h-0.5 bg-[#29465d] transition-all duration-300 ${menuOpen ? 'rotate-45 translate-y-2' : ''}`}></span>
              <span className={`block w-6 h-0.5 bg-[#29465d] transition-all duration-300 ${menuOpen ? 'opacity-0' : ''}`}></span>
              <span className={`block w-6 h-0.5 bg-[#29465d] transition-all duration-300 ${menuOpen ? '-rotate-45 -translate-y-2' : ''}`}></span>
            </button>
          )}
        </div>
      </div>

      {activeMenu !== "Login" && activeMenu !== "Register" && (
        <div className={`lg:hidden fixed top-14 left-0 w-full z-10 backdrop-blur-md bg-black/20 transition-all duration-300 overflow-hidden ${menuOpen ? 'max-h-screen py-4' : 'max-h-0'}`}>
          <div className='flex flex-col items-center gap-5 text-lg font-semibold px-6 pb-4'>
            
            {accessToken && (
              <div className='flex items-center gap-3 pt-2'>
                <p className='font-bold text-[#FC563C]'>
                  {t('header.balance')}: <span className='text-[18px]'>₼{userBalance}</span>
                </p>
                <div onClick={() => { navigate('/my-wallet'); setMenuOpen(false) }} className='bg-[#172A39] text-white rounded-md w-5 h-5 cursor-pointer border-2px hover:bg-[#FC563C] hover:border-[#172A39] border-[#FC563C] flex justify-center items-center p-3 transition-all duration-300'>+</div>
              </div>
            )}

            <Link
              to='/'
              onClick={() => setMenuOpen(false)}
              className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "Home" ? "text-[#FC563C]" : "text-[#6E7575]"}`}
            >
              {t('header.home')}
              <span className={`absolute left-0 -bottom-1 h-1 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Home" ? "w-full" : "w-0"}`}></span>
            </Link>

            <button
              onClick={() => { checkAccess(); setMenuOpen(false) }}
              className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "Reservation" ? "text-[#FC563C]" : "text-[#6E7575]"}`}
            >
              {t('header.reservations')}
              <span className={`absolute left-0 -bottom-1 h-1 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Reservation" ? "w-full" : "w-0"}`}></span>
            </button>

            <Link
              to='/about'
              onClick={() => setMenuOpen(false)}
              className={`relative cursor-pointer transition-all duration-500 ${activeMenu === "About" ? "text-[#FC563C]" : "text-[#6E7575]"}`}
            >
              {t('header.aboutUs')}
              <span className={`absolute left-0 -bottom-1 h-1 bg-[#FC563C] transition-all duration-500 ${activeMenu === "About" ? "w-full" : "w-0"}`}></span>
            </Link>

            <Link to='/contact' className={`list-none relative cursor-pointer transition-all duration-500 ${activeMenu === "Contact" ? "text-[#FC563C]" : "text-[#6E7575]"}`}>
              {t('header.contact')}
              <span className={`absolute left-0 -bottom-1 h-1 bg-[#FC563C] transition-all duration-500 ${activeMenu === "Contact" ? "w-full" : "w-0"}`}></span>
            </Link>

            {!accessToken && (
              <button
                onClick={() => { navigate('/login'); setMenuOpen(false) }}
                className="bg-gradient-to-r from-[#FC563C] to-[#FF8A70] text-white px-6 py-2 rounded-lg font-semibold cursor-pointer transition-all duration-300 hover:scale-105 mt-2"
              >
                {t('header.getStarted')}
              </button>
            )}
          </div>
        </div>
      )}
    </>
  )
}

export default Header