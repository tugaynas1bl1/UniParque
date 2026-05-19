import React, { useEffect, useState } from "react";
import { Mail, Phone, MapPin, Send } from "lucide-react";
import api from "../utils/axios";
import toast from "react-hot-toast";
import instagram from "../assets/instagram.png";
import facebook from "../assets/facebook.png";
import github from "../assets/github.png";
import linkedin from "../assets/linkedin.png";
import { useTranslation } from "react-i18next";

const Contact = () => {
  const { t } = useTranslation();

  const [form, setForm] = useState({
    name: "",
    email: "",
    message: "",
  });

  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      setLoading(true);

      await api.post("/auth/send-email-message", {
        name: form.name,
        email: form.email,
        message: form.message,
      });

      toast.success(t("contact.success"));
      setForm({ name: "", email: "", message: "" });

    } catch (err) {
      toast.error(t("contact.error"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    document.body.style.overflow = "auto"
    window.scrollTo({ top: 0, behavior: "smooth" })
  }, [])

  return (
    <div className="min-h-screen bg-gradient-to-br from-black via-gray-900 to-gray-800 text-white px-4 py-12">
      
      <div className="animate-[comeFromTop_1s] text-center mb-12">
        <h1 className="text-4xl md:text-5xl font-bold mb-4 mt-10">
          {t("contact.title")}
        </h1>
        <p className="text-gray-400 max-w-xl mx-auto">
          {t("contact.subtitle")}
        </p>
      </div>

      <div className="max-w-6xl mx-auto grid md:grid-cols-2 gap-10">
        

        <div className="space-y-6">
          
          <div className="animate-[scaleFromTop_1s] flex items-center gap-4 p-6 rounded-2xl bg-white/5 backdrop-blur-xl border border-white/10 hover:scale-[1.03] transition">
            <Mail className="text-orange-500" />
            <div>
              <h3 className="font-semibold">{t("contact.email")}</h3>
              <p className="text-gray-400">support@uniparque.com</p>
            </div>
          </div>

          <div className="animate-[scaleFromTop_2s] flex items-center gap-4 p-6 rounded-2xl bg-white/5 backdrop-blur-xl border border-white/10 hover:scale-[1.03] transition">
            <Phone className="text-green-500" />
            <div>
              <h3 className="font-semibold">{t("contact.phone")}</h3>
              <p className="text-gray-400">+994 51 303 48 48</p>
            </div>
          </div>

          <div className="animate-[scaleFromTop_2.5s] flex items-center gap-4 p-6 rounded-2xl bg-white/5 backdrop-blur-xl border border-white/10 hover:scale-[1.03] transition">
            <MapPin className="text-blue-500" />
            <div>
              <h3 className="font-semibold">{t("contact.location")}</h3>
              <p className="text-gray-400">{t("contact.locationText")}</p>
            </div>
          </div>

        </div>

        <div className="animate-[revealDown_2s] p-8 rounded-2xl bg-white/5 backdrop-blur-2xl border border-white/10 shadow-xl">
          
          <h2 className="text-2xl font-semibold mb-6">
            {t("contact.sendMessage")}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-6">
            
            {/* NAME */}
            <div className="relative">
              <input
                type="text"
                name="name"
                required
                placeholder=" "
                value={form.name}
                onChange={handleChange}
                className="peer w-full p-4 bg-transparent border border-gray-600 rounded-xl outline-none focus:border-orange-500"
              />
              <label className="absolute left-3 -top-2 text-xs text-orange-500 
                transition-all duration-200
                peer-placeholder-shown:top-4 
                peer-placeholder-shown:text-base 
                peer-placeholder-shown:text-gray-400
                peer-focus:-top-2 peer-focus:text-xs peer-focus:text-orange-500
                bg-gray-900 px-2 rounded">
                {t("contact.name")}
              </label>
            </div>

            {/* EMAIL */}
            <div className="relative">
              <input
                type="email"
                name="email"
                required
                placeholder=" "
                value={form.email}
                onChange={handleChange}
                className="peer w-full p-4 bg-transparent border border-gray-600 rounded-xl outline-none focus:border-orange-500"
              />
              <label className="absolute left-3 -top-2 text-xs text-orange-500 
                transition-all duration-200
                peer-placeholder-shown:top-4 
                peer-placeholder-shown:text-base 
                peer-placeholder-shown:text-gray-400
                peer-focus:-top-2 peer-focus:text-xs peer-focus:text-orange-500
                bg-gray-900 px-2 rounded">
                {t("contact.emailInput")}
              </label>
            </div>

            {/* MESSAGE */}
            <div className="relative">
              <textarea
                name="message"
                rows="4"
                required
                placeholder=" "
                value={form.message}
                onChange={handleChange}
                className="peer w-full p-4 bg-transparent border border-gray-600 rounded-xl outline-none focus:border-orange-500 resize-none"
              />
              <label className="absolute left-3 -top-2 text-xs text-orange-500 
                transition-all duration-200
                peer-placeholder-shown:top-4 
                peer-placeholder-shown:text-base 
                peer-placeholder-shown:text-gray-400
                peer-focus:-top-2 peer-focus:text-xs peer-focus:text-orange-500
                bg-gray-900 px-2 rounded">
                {t("contact.message")}
              </label>
            </div>

            {/* BUTTON */}
            <button
              type="submit"
              disabled={loading}
              className="cursor-pointer hover:opacity-80 w-full flex items-center justify-center gap-2 p-4 rounded-xl font-semibold 
              bg-gradient-to-r from-orange-500 hover:oran to-orange-600 hover:scale-[1.03] transition-all duration-300"
            >
              {loading ? t("contact.sending") : <>
                {t("contact.send")} <Send size={18} />
              </>}
            </button>

          </form>

          {/* SOCIALS (unchanged UI) */}
          <div className="flex justify-center gap-4 mt-6 text-gray-400">
            <a href="https://www.instagram.com/tugainasibly" className="hover:rotate-405 hover:scale-130 transition-all duration-500">
              <img src={instagram} />
            </a>
            <a href="https://www.facebook.com/tuqay.nasibli" className="hover:rotate-405 hover:scale-130 transition-all duration-500">
              <img src={facebook} />
            </a>
            <a href="https://github.com/tugaynas1bl1" className="hover:rotate-405 hover:scale-130 transition-all duration-500">
              <img src={github} />
            </a>
            <a href="#" className="hover:rotate-405 hover:scale-130 transition-all duration-500">
              <img src={linkedin} />
            </a>
          </div>

        </div>
      </div>
    </div>
  );
};

export default Contact;