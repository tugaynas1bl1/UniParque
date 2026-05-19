import React from "react";
import { Mail, MapPin, Phone } from "lucide-react";
import logo from "../assets/Logo.png";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const Footer = () => {
  const { t } = useTranslation();

  return (
    <footer className="bg-gradient-to-br from-gray-950 via-gray-900 to-black text-white border-t border-white/10">

      <div className="max-w-7xl mx-auto px-6 py-14 grid grid-cols-1 md:grid-cols-3 gap-10">

        {/* BRAND */}
        <div>
          <div className="flex items-center gap-2">
            <img
              className="w-10 h-10 lg:w-13 lg:h-13 xl:w-15 xl:h-15 img-outline"
              src={logo}
              alt="logo"
            />

            <p className="font-bold text-[16px] lg:text-[17px] xl:text-[20px]">
              <span className="text-[#FC563C]">Uni</span>Parque
            </p>
          </div>

          <p className="text-gray-400 w-70 text-sm leading-relaxed mt-3">
            {t("footer.description")}
          </p>
        </div>

        {/* LINKS */}
        <div>
          <h2 className="text-lg font-semibold mb-4">
            {t("footer.quickLinks")}
          </h2>

          <div className="flex flex-col gap-3 text-gray-400 text-sm">
            <Link to='/' className="hover:text-white cursor-pointer transition">
              {t("footer.home")}
            </Link>

            <Link Link to='/about' className="hover:text-white cursor-pointer transition">
              {t("footer.about")}
            </Link>

            <Link Link to='/reservation' className="hover:text-white cursor-pointer transition">
              {t("footer.reservations")}
            </Link>

            <Link Link to='/contact' className="hover:text-white cursor-pointer transition">
              {t("footer.contact")}
            </Link>
          </div>
        </div>

        <div>
          <h2 className="text-lg font-semibold mb-4">
            {t("footer.contactTitle")}
          </h2>

          <div className="space-y-4 text-gray-400 text-sm">

            <div className="flex items-center gap-2">
              <Mail size={16} className="text-orange-500" />
              support@uniparque.com
            </div>

            <div className="flex items-center gap-2">
              <Phone size={16} className="text-green-500" />
              +994 51 303 48 48
            </div>

            <div className="flex items-center gap-2">
              <MapPin size={16} className="text-blue-500" />
              Baku, Azerbaijan
            </div>

          </div>
        </div>

      </div>

      <div className="border-t border-white/10 py-5 text-center text-gray-500 text-sm">
        © {new Date().getFullYear()} UniParque. {t("footer.rights")}
      </div>

    </footer>
  );
};

export default Footer;