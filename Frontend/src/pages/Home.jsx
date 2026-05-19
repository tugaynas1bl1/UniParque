import React, { useEffect } from 'react'
import heroImg from '../assets/Parking.jpg'
import map from '../assets/map.png'
import phone from '../assets/App.png'
import ScrollReveal from 'scrollreveal'
import { Link } from 'react-router-dom'
import { useTokens } from '../stores/tokenStore'
import payment from '../assets/payment.png'
import reserve from '../assets/reserve.png'
import { useTranslation } from 'react-i18next'

const Home = () => {
  const { t } = useTranslation()
  const { accessToken } = useTokens()

  useEffect(() => {
    document.body.style.overflow = "auto"
    window.scrollTo({ top: 0, behavior: "smooth" })

    const sr = ScrollReveal({
      reset: false,
      distance: '200px',
      origin: 'top',
      duration: 500,
      easing: 'ease-in-out',
      viewFactor: 0.4
    })

    sr.reveal('.sec1', { delay: 100 })
    sr.reveal('.sec3', { delay: 100 })
    sr.reveal('.sec4', { delay: 100 })
    sr.reveal('.sec5', { delay: 100 })
    sr.reveal('.sec6', { delay: 100 })
    sr.reveal('.sec7', { delay: 100 })
    sr.reveal('.sec8', { delay: 100 })
    sr.reveal('.feature2', { delay: 500, origin: 'bottom' })
    sr.reveal('.feature1', { delay: 500, origin: 'left' })
    sr.reveal('.feature3', { delay: 500, origin: 'right' })
    sr.reveal('.feature-header', { delay: 500 })
    sr.reveal('.cta-header', { delay: 500 })
    sr.reveal('.cta-p', { delay: 500, origin: 'right' })
    sr.reveal('.cta-btn', { delay: 500, origin: 'left' })
    sr.reveal('.hiw-header', { delay: 500, origin: 'bottom' })
    sr.reveal('.hiw-div1', { delay: 1000, origin: 'left' })
    sr.reveal('.hiw-div2', { delay: 700, origin: 'left' })
    sr.reveal('.hiw-div3', { delay: 400, origin: 'left' })
    sr.reveal('.stat-1', { delay: 1000, origin: 'left' })
    sr.reveal('.stat-2', { delay: 700, origin: 'left' })
    sr.reveal('.stat-3', { delay: 400, origin: 'left' })
    sr.reveal('.sec6 h2', { delay: 400, origin: 'left' })
    sr.reveal('.sec6 p', { delay: 800, origin: 'left' })
    sr.reveal('.sec6 button', { delay: 1200, origin: 'left' })
    sr.reveal('.sec6 img', { delay: 600, origin: 'right' })
  }, [])

  return (
    <div className="w-full overflow-hidden bg-[#0B0F14] text-white">
      
      <section className="sec1 relative w-full min-h-screen flex items-center justify-center px-4 sm:px-5 md:px-6">
        <img src={heroImg} className="absolute inset-0 w-full h-full object-cover opacity-20" alt="" />
        <div className="absolute inset-0 bg-gradient-to-b from-black via-[#0B0F14]/90 to-[#0B0F14]"></div>
        <div className="relative z-10 max-w-5xl text-center space-y-4 sm:space-y-5 md:space-y-6">
          <h1 className="text-3xl sm:text-4xl md:text-5xl lg:text-7xl font-bold leading-tight">
            <span className="text-[#FC563C]">{t('heroHighlight')}</span> {t('heroTitle').replace('Smart Parking', '')}
          </h1>
          <p className="text-gray-300 text-sm sm:text-base md:text-lg max-w-2xl mx-auto">
            {t('heroDescription')}
          </p>
          <div className="flex justify-center gap-3 sm:gap-4 md:gap-6 flex-wrap mt-4 sm:mt-5 md:mt-6">
            <Link to='/create-reservation' className="cursor-pointer bg-[#FC563C] px-5 sm:px-6 md:px-8 py-2.5 sm:py-3 rounded-lg text-sm sm:text-base font-semibold hover:bg-white hover:text-[#FC563C] transition-all duration-300">
              {t('heroStartButton')}
            </Link>
            <Link to='/about' className="cursor-pointer border border-white px-5 sm:px-6 md:px-8 py-2.5 sm:py-3 rounded-lg text-sm sm:text-base font-semibold hover:bg-[#FC563C] hover:border-[#FC563C] transition-all duration-300">
              {t('heroLearnMoreButton')}
            </Link>
          </div>
        </div>
      </section>

      <section className="sec2 py-12 sm:py-16 md:py-20 lg:py-24 px-4 sm:px-5 md:px-6 mx-auto text-center cursor-pointer">
        <h2 className="feature-header text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold mb-8 sm:mb-10 md:mb-12 lg:mb-16">
          {t('featureHeader')}
        </h2>
        <div className="grid md:grid-cols-3 gap-5 sm:gap-7 md:gap-8 lg:gap-10">
          <div className="feature1 bg-[#11161D] p-5 sm:p-6 md:p-7 lg:p-8 rounded-2xl hover:scale-105 transition border border-white/5">
            <img src={map} className="w-36 sm:w-44 md:w-52 lg:w-60 mx-auto mb-3 md:mb-4" />
            <h3 className="text-base sm:text-lg md:text-xl font-semibold text-[#FC563C]">{t('feature1Title')}</h3>
            <p className="text-gray-400 mt-2 text-sm sm:text-sm md:text-base">{t('feature1Description')}</p>
          </div>
          <div className="feature2 bg-[#11161D] p-5 sm:p-6 md:p-7 lg:p-8 rounded-2xl hover:scale-105 transition border border-white/5">
            <img src={reserve} className="w-24 sm:w-28 md:w-32 lg:w-35 mx-auto mb-3 md:mb-4 rounded-lg" />
            <h3 className="text-base sm:text-lg md:text-xl font-semibold text-[#FC563C]">{t('feature2Title')}</h3>
            <p className="text-gray-400 mt-2 text-sm sm:text-sm md:text-base">{t('feature2Description')}</p>
          </div>
          <div className="feature3 bg-[#11161D] p-5 sm:p-6 md:p-7 lg:p-8 rounded-2xl hover:scale-105 transition border border-white/5">
            <img src={payment} className="w-28 sm:w-32 md:w-36 lg:w-40 mx-auto mb-3 md:mb-4" />
            <h3 className="text-base sm:text-lg md:text-xl font-semibold text-[#FC563C]">{t('feature3Title')}</h3>
            <p className="text-gray-400 mt-2 text-sm sm:text-sm md:text-base">{t('feature3Description')}</p>
          </div>
        </div>
      </section>

      <section className="sec3 py-12 sm:py-16 md:py-20 lg:py-24 bg-linear-to-t from-[#fb2c0c] to-[#ec735e] text-center text-white px-4 sm:px-5">
        <h2 className="cta-header text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold mb-3 md:mb-4">{t('ctaHeader')}</h2>
        <p className="cta-p mb-5 md:mb-8 text-sm sm:text-base md:text-base lg:text-lg">{t('ctaDescription')}</p>
        {!accessToken && (
          <Link to='/login' className="cta-btn bg-[#11161D] cursor-pointer hover:bg-white hover:text-[#FC563C] text-white px-7 sm:px-8 md:px-10 py-3 md:py-4 rounded-xl text-sm sm:text-base font-semibold hover:scale-105 transition">
            {t('ctaButton')}
          </Link>
        )}
      </section>

      <section className="sec4 py-12 sm:py-16 md:py-20 lg:py-24 px-4 sm:px-5 md:px-6 text-center bg-white text-black">
        <div className="max-w-6xl mx-auto">
          <h2 className="hiw-header text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold mb-8 sm:mb-10 md:mb-12 lg:mb-16">{t('hiwHeader')}</h2>
          <div className="grid md:grid-cols-3 gap-7 sm:gap-9 md:gap-10 lg:gap-12">
            <div className='hiw-div1'>
              <div className="text-4xl sm:text-4xl md:text-5xl font-bold text-[#FC563C] mb-3 md:mb-4">1</div>
              <h3 className="text-base sm:text-lg md:text-xl font-semibold mb-2">{t('hiwStep1Title')}</h3>
              <p className="text-gray-600 text-sm sm:text-sm md:text-base">{t('hiwStep1Description')}</p>
            </div>
            <div className='hiw-div2'>
              <div className="text-4xl sm:text-4xl md:text-5xl font-bold text-[#FC563C] mb-3 md:mb-4">2</div>
              <h3 className="text-base sm:text-lg md:text-xl font-semibold mb-2">{t('hiwStep2Title')}</h3>
              <p className="text-gray-600 text-sm sm:text-sm md:text-base">{t('hiwStep2Description')}</p>
            </div>
            <div className='hiw-div3'>
              <div className="text-4xl sm:text-4xl md:text-5xl font-bold text-[#FC563C] mb-3 md:mb-4">3</div>
              <h3 className="text-base sm:text-lg md:text-xl font-semibold mb-2">{t('hiwStep3Title')}</h3>
              <p className="text-gray-600 text-sm sm:text-sm md:text-base">{t('hiwStep3Description')}</p>
            </div>
          </div>
        </div>
      </section>

      <section className="sec5 py-12 sm:py-14 md:py-16 lg:py-20 bg-white text-center text-black px-4 sm:px-5">
        <div className="max-w-6xl mx-auto grid grid-cols-2 md:grid-cols-4 gap-6 sm:gap-8 md:gap-10">
          <div className='stat-1'>
            <h3 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold text-[#FC563C]">{t('stat1Value')}</h3>
            <p className="text-gray-600 text-sm sm:text-sm md:text-base mt-1">{t('stat1Label')}</p>
          </div>
          <div className='stat-2'>
            <h3 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold text-[#FC563C]">{t('stat2Value')}</h3>
            <p className="text-gray-600 text-sm sm:text-sm md:text-base mt-1">{t('stat2Label')}</p>
          </div>
          <div className='stat-3'>
            <h3 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold text-[#FC563C]">{t('stat3Value')}</h3>
            <p className="text-gray-600 text-sm sm:text-sm md:text-base mt-1">{t('stat3Label')}</p>
          </div>
          <div className='stat-4'>
            <h3 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold text-[#FC563C]">{t('stat4Value')}</h3>
            <p className="text-gray-600 text-sm sm:text-sm md:text-base mt-1">{t('stat4Label')}</p>
          </div>
        </div>
      </section>

      <section className="sec6 py-12 sm:py-16 md:py-20 lg:py-24 px-4 sm:px-5 md:px-6 bg-white text-black">
        <div className="max-w-6xl mx-auto flex flex-col md:flex-row items-center gap-8 sm:gap-10 md:gap-12 lg:gap-16">
          <div className="flex-1 text-center md:text-left">
            <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold mb-4 md:mb-6">{t('appHeader')}</h2>
            <p className="text-gray-600 mb-4 md:mb-6 text-sm sm:text-base">{t('appDescription')}</p>
            <button className="bg-[#FC563C] cursor-pointer px-6 sm:px-7 md:px-8 py-2.5 sm:py-3 rounded-lg text-sm sm:text-base font-semibold text-white hover:bg-black hover:text-white transition">
              {t('appButton')}
            </button>
          </div>
          <div className="flex-1 w-full">
            <img src={phone} className="w-full h-fit max-w-xs sm:max-w-sm md:max-w-md mx-auto" />
          </div>
        </div>
      </section>

      <section className="sec7 py-12 sm:py-16 md:py-20 lg:py-24 bg-[#0F141A] text-center px-4 sm:px-5 md:px-6">
        <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-bold mb-8 sm:mb-10 md:mb-12 lg:mb-16">{t('testimonialsHeader')}</h2>
        <div className="grid md:grid-cols-3 gap-5 sm:gap-6 md:gap-8 lg:gap-10 max-w-6xl mx-auto">
          <div className="bg-[#1A2129] p-4 sm:p-5 md:p-6 rounded-xl">
            <p className="text-gray-400 text-sm sm:text-sm md:text-base">{t('testimonial1Text')}</p>
            <h4 className="mt-3 md:mt-4 text-[#FC563C] font-semibold text-sm sm:text-base">{t('testimonial1Author')}</h4>
          </div>
          <div className="bg-[#1A2129] p-4 sm:p-5 md:p-6 rounded-xl">
            <p className="text-gray-400 text-sm sm:text-sm md:text-base">{t('testimonial2Text')}</p>
            <h4 className="mt-3 md:mt-4 text-[#FC563C] font-semibold text-sm sm:text-base">{t('testimonial2Author')}</h4>
          </div>
          <div className="bg-[#1A2129] p-4 sm:p-5 md:p-6 rounded-xl">
            <p className="text-gray-400 text-sm sm:text-sm md:text-base">{t('testimonial3Text')}</p>
            <h4 className="mt-3 md:mt-4 text-[#FC563C] font-semibold text-sm sm:text-base">{t('testimonial3Author')}</h4>
          </div>
        </div>
      </section>

    </div>
  )
}

export default Home