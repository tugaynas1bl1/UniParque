import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import ScrollReveal from "scrollreveal";

const About = () => {
  const { t } = useTranslation();

  const features = [
    {
      title: t("about.features.realTimeReservations.title"),
      desc: t("about.features.realTimeReservations.desc"),
    },
    {
      title: t("about.features.smartManagement.title"),
      desc: t("about.features.smartManagement.desc"),
    },
    {
      title: t("about.features.multipleLocations.title"),
      desc: t("about.features.multipleLocations.desc"),
    },
    {
      title: t("about.features.fastModernUI.title"),
      desc: t("about.features.fastModernUI.desc"),
    },
    {
      title: t("about.features.secureSystem.title"),
      desc: t("about.features.secureSystem.desc"),
    },
    {
      title: t("about.features.optimizedPerformance.title"),
      desc: t("about.features.optimizedPerformance.desc"),
    },
  ];

  const stats = [
    { value: t("about.stats.reservations.value"), label: t("about.stats.reservations.label") },
    { value: t("about.stats.activeUsers.value"), label: t("about.stats.activeUsers.label") },
    { value: t("about.stats.parkingZones.value"), label: t("about.stats.parkingZones.label") },
    { value: t("about.stats.satisfactionRate.value"), label: t("about.stats.satisfactionRate.label") },
  ];

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
      sr.reveal('.sec2', { delay: 100 })
      sr.reveal('.sec3', { delay: 100 })
      sr.reveal('.sec4', { delay: 500, origin: 'bottom' })
      sr.reveal('.sec5', { delay: 500, origin: 'left' })
      sr.reveal('.sec6', { delay: 500, origin: 'right' })

    }, [])

return (
  <div className="w-full overflow-x-hidden bg-gray-900 text-white px-3 sm:px-4 md:px-6 py-8 sm:py-10 md:py-12">

    <section className="sec1 text-center max-w-4xl mx-auto mb-12 md:mb-16 mt-15">
      <h1 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold mb-4">
        {t("about.hero.title")}
      </h1>
      <p className="text-gray-400 text-sm sm:text-base md:text-lg">
        {t("about.hero.desc")}
      </p>
    </section>

    <section className="sec2 max-w-5xl mx-auto mb-10 md:mb-16">
      <h2 className="text-xl sm:text-2xl font-semibold mb-3">
        🚧 {t("about.problem.title")}
      </h2>
      <p className="text-gray-400 text-sm sm:text-base leading-6 sm:leading-7">
        {t("about.problem.desc")}
      </p>
    </section>

    <section className="sec3 max-w-5xl mx-auto mb-10 md:mb-16">
      <h2 className="text-xl sm:text-2xl font-semibold mb-3">
        💡 {t("about.solution.title")}
      </h2>
      <p className="text-gray-400 text-sm sm:text-base leading-6 sm:leading-7">
        {t("about.solution.desc")}
      </p>
    </section>

    <section className="sec4 max-w-6xl mx-auto mb-12 md:mb-20">
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-5 md:gap-6">
        {features.map((f, i) => (
          <div key={i} className="bg-gray-800 p-5 rounded-2xl hover:scale-105 transition">
            <h3 className="text-base sm:text-lg font-semibold mb-2">{f.title}</h3>
            <p className="text-gray-400 text-xs sm:text-sm">{f.desc}</p>
          </div>
        ))}
      </div>
    </section>

    <section className="sec5 max-w-5xl mx-auto mb-12 md:mb-20 text-center">
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {stats.map((s, i) => (
          <div key={i} className="bg-gray-800 p-4 rounded-xl">
            <p className="text-indigo-400 font-bold text-xl sm:text-2xl">
              {s.value}
            </p>
            <p className="text-gray-400 text-xs sm:text-sm">
              {s.label}
            </p>
          </div>
        ))}
      </div>
    </section>

    <section className="sec6 text-center max-w-3xl mx-auto">
      <h2 className="text-xl sm:text-2xl font-semibold mb-3">
        {t("about.cta.title")}
      </h2>

      <p className="text-gray-400 text-sm sm:text-base mb-5">
        {t("about.cta.desc")}
      </p>

      <Link
        to="/create-reservation"
        className="inline-block bg-indigo-600 hover:bg-indigo-500 px-6 py-3 rounded-xl font-semibold"
      >
        {t("about.cta.button")}
      </Link>
    </section>

  </div>
);
};

export default About;