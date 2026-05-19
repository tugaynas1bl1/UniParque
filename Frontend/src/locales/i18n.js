import { useTranslation, initReactI18next } from "react-i18next";
import i18n from "i18next";
import * as en from "./en.json"
import * as az from "./az.json"
import * as ru from "./ru.json"

export const languages = [
    {
        title: "English",
        value: "en"
    },
    {
        title: "Azərbaycan dili",
        value: "az"
    },
    {
        title: "Pусский язык",
        value: "ru"
    }
]

i18n
  .use(initReactI18next) // passes i18n down to react-i18next
  .init({
    // the translations
    // (tip move them in a JSON file and import them,
    // or even better, manage them via a UI: https://react.i18next.com/guides/multiple-translation-files#manage-your-translations-with-a-management-gui)
    resources: {
      en: {
        translation: en
      },
      az: {
        
        translation: az
      },
      ru: {
        
        translation: ru
      }
    },
    lng: "en", // if you're using a language detector, do not define the lng option
    fallbackLng: "en",

    interpolation: {
      escapeValue: false // react already safes from xss => https://www.i18next.com/translation-function/interpolation#unescape
    }
  });