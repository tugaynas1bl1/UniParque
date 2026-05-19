import { useTranslation } from 'react-i18next';
import { languages } from '../locales/i18n';
import { useEffect } from 'react';

const LanguageSelector = () => {
  const { i18n } = useTranslation();

  useEffect(() => {
    const savedLanguage = localStorage.getItem('language');
    if (savedLanguage && savedLanguage !== i18n.language) {
      i18n.changeLanguage(savedLanguage);
    }
  }, [i18n]);

  const changeLanguage = (event) => {
    const newLanguage = event.target.value;
    i18n.changeLanguage(newLanguage);
    localStorage.setItem('language', newLanguage);
  };

  return (
    <select
      className='border-1 rounded-4xl border-[#FC563C] cursor-pointer bg-gradient-to-r from-[#7a1500] to-[#FC563C] outline-none lg:text-[14px] xl:text-lg text-white px-3 h-[30px] text-[15px] w-[70px] lg:w-fit lg:ml-40 lg:mt-5 lg:h-[30px] xl:h-[40px]'
      onChange={changeLanguage}
      value={i18n.language}
    >
      {languages.map(language => (
        <option className='bg-gray-950 rounded text-white' key={language.value} value={language.value}>
          {language.title}
        </option>
      ))}
    </select>
  );
};

export default LanguageSelector;