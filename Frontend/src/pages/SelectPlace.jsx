import React, { useEffect, useState } from 'react';
import { useParams, useLocation } from 'react-router-dom';
import api from '../utils/axios';
import ReservationDetails from '../components/ReservationDetails';
import carOrange from '../assets/carOrange.png';
import carGreen from '../assets/carGreen.png';
import reserved from '../assets/Reserved.png';
import reservedGreen from '../assets/reserved-green.png';
import toast from 'react-hot-toast';
import { useTranslation } from 'react-i18next';

const SelectPlace = () => {
  const { t } = useTranslation();
  const { branchId } = useParams();
  const location = useLocation();
  const isAdmin = location.pathname.startsWith('/admin');

  const [fullLayout, setFullLayout] = useState(null);
  const [isCardHidden, setIsCardHidden] = useState(true);
  const [isLegendVisible, setIsLegendVisible] = useState(false);
  const [selected, setSelected] = useState({
    branchId: '',
    branch: '',
    section: '',
    subSection: '',
    place: null,
  });

  const getSections = async () => {
    try {
      const { data, statusText } = await api.get(`ParkingBranches/full-layout/${branchId}`);
      if (statusText === 'OK' && data) setFullLayout(data.data || []);
    } catch (error) {
      console.error(error);
    }
  };

  const handlePlaceReserved = ({ placeId, isReserved }) => {
    setFullLayout(prev => {
      if (!prev) return prev;
      return {
        ...prev,
        sections: prev.sections.map(section => ({
          ...section,
          subSections: section.subSections.map(sub => ({
            ...sub,
            places: sub.places.map(p => p.id === placeId ? { ...p, isReserved } : p),
          })),
        })),
      };
    });
  };


  useEffect(() => { getSections(); }, [branchId]);

  const placeClick = (branchName, sectionName, subSectionName, place) => {
    if (isAdmin) return;
    if (place.isReserved) { toast.error(t('selectPlace.reservedByAnother')); return; }
    if (place.isOccupied) { toast.error(t('selectPlace.occupiedByAnother')); return; }
    setSelected({ branchId, branch: branchName, section: sectionName, subSection: subSectionName, place });
    setIsCardHidden(false);
  };

  if (!fullLayout) {
    return (
      <p className="h-screen flex justify-center items-center w-full mt-20 text-center text-gray-500">
        {t('selectPlace.loading')}
      </p>
    );
  }

  if (!fullLayout.sections || fullLayout.sections.length === 0) {
    return (
      <p className="h-screen flex justify-center items-center w-full mt-20 text-center text-gray-500">
        {t('selectPlace.emptyBranch')}
      </p>
    );
  }

  return (
    <div className="min-h-screen bg-[#0B0F14] text-white px-3 sm:px-4 md:px-6 py-6 md:py-10 mt-10 md:mt-15 relative">
      
      <h1 className="text-xl sm:text-2xl md:text-3xl lg:text-4xl font-bold text-center mb-2 animate-[comeFromSection_1s]">
        "{fullLayout.branchName}" {t('selectPlace.parkingLayout')}
      </h1>

      <div className="relative flex flex-col gap-3 justify-center items-center mb-6 md:mb-10">
        <div
          onMouseEnter={() => setIsLegendVisible(true)}
          onMouseLeave={() => setIsLegendVisible(false)}
          className="cursor-pointer w-6 h-6 flex items-center justify-center rounded-full bg-[#FC563C] text-white text-sm font-semibold"
        >
          i
        </div>

        <div
          className={`absolute top-11 left-1/2 -translate-x-1/2 max-w-xs sm:max-w-md md:max-w-3xl text-xs sm:text-sm text-gray-300 bg-[#11161D] p-3 sm:p-4 rounded-xl border border-white/10 transition-all duration-300 ease-out pointer-events-none z-50 ${
            isLegendVisible ? 'opacity-100 scale-100 translate-y-0' : 'opacity-0 scale-0 -translate-y-20'
          }`}
        >
          <div className="absolute -top-2 left-1/2 -translate-x-1/2 w-3 h-3 bg-[#FC563C] rotate-45 rounded-sm border-t border-l border-white/10" />
          <p className="mb-2 font-semibold text-[#FC563C]">{t('selectPlace.legend')}</p>
          <ul className="space-y-1">
            <li>🟢 {t('selectPlace.parkedByYou')}</li>
            <li>🟢 {t('selectPlace.reservedByYou')}</li>
            <li>🔴 {t('selectPlace.reservedByAnotherUser')}</li>
            <li>🔴 {t('selectPlace.occupiedByAnotherUser')}</li>
            <li>⚪ {t('selectPlace.available')}</li>
          </ul>
        </div>
      </div>

      {!isCardHidden && (
        <div className="absolute w-full h-full">
          <ReservationDetails bookedData={selected} onClose={() => setIsCardHidden(true)} />
        </div>
      )}

      <div className="flex flex-col gap-10">

        {fullLayout.sections.map((section) => (
          <div key={section.id} className="w-full">

            <div className="relative animate-[comeFromLeft_1s] text-xl sm:text-2xl md:text-3xl lg:text-5xl font-semibold mb-4 text-center text-white bg-[#FC563C] flex justify-center items-center rounded-t-xl h-40 sm:h-32 md:h-40">
              {section.name}
            </div>

            <div className="backdrop-blur-xl p-3 sm:p-4 md:p-5 rounded-b-2xl border-r-4 border-[#FC563C] animate-[comeFromSection_1s]">

              <div className="flex flex-wrap gap-6 justify-center items-start">

                {!section.subSections || section.subSections.length === 0 ? (
                  <p className="text-gray-400 text-xs sm:text-sm md:text-base text-center w-full py-4">
                    {t('selectPlace.noSubSections')}
                  </p>
                ) : (
                  section.subSections.map((sub) => (
                    <div
                      key={sub.id}
                      className="flex flex-col px-3 sm:px-6 md:px-10 border-r border-white/10 last:border-0"
                    >
                      <h3 className="text-sm sm:text-lg md:text-xl text-center mb-2">
                        {sub.name}
                      </h3>

                      {!sub.places || sub.places.length === 0 ? (
                        <p className="text-gray-400 text-xs sm:text-sm md:text-base text-center py-4">
                          {t('selectPlace.noPlaces')}
                        </p>
                      ) : (
                        <div className="flex flex-wrap justify-center gap-1 sm:gap-2 mt-2">

                          {sub.places.map((place) => (
                            <div
                              key={place.id}
                              onClick={() =>
                                placeClick(fullLayout.branchName, section.name, sub.name, place)
                              }
                              className={`${
                                place.isOccupied && !place.isReservedByMe
                                  ? 'border-[#FC563C]'
                                  : place.isOccupied && place.isReservedByMe
                                  ? 'border-green-400'
                                  : place.isReservedByMe
                                  ? 'border-green-600'
                                  : place.isReserved
                                  ? 'border-[#FC563C]'
                                  : 'border-white'
                              } ${
                                place.isOccupied && !place.isReservedByMe
                                  ? 'bg-[#fc563c2c]'
                                  : place.isOccupied && place.isReservedByMe
                                  ? 'bg-green-500/40'
                                  : ''
                              } w-10 h-16 sm:w-12 sm:h-20 md:w-16 md:h-24 border-l-4 border-r-4 flex items-center justify-center hover:border-[#FC563C] transition cursor-pointer`}
                            >
                              <div className="h-full border-white border-l-2 border-dashed"></div>

                              {place.isReserved && !place.isReservedByMe && (
                                <img
                                  className="absolute w-10 h-10 md:w-17 md:h-15 opacity-0 animate-[stampEffect_0.3s_forwards] [animation-delay:1.3s]"
                                  src={reserved}
                                />
                              )}

                              {!place.isOccupied && place.isReservedByMe && (
                                <img
                                  className="absolute opacity-0 h-11 lg:h-15 animate-[stampEffect_0.3s_forwards] [animation-delay:1.3s]"
                                  src={reservedGreen}
                                />
                              )}

                              {place.isOccupied && !place.isReservedByMe && (
                                <img
                                  className="absolute rotate-270 w-20 h-13 opacity-0 animate-[carMoveEffect_1s_forwards] [animation-delay:1.3s]"
                                  src={carOrange}
                                />
                              )}

                              {place.isOccupied && place.isReservedByMe && (
                                <img
                                  className="absolute h-16 w-16 lg:h-20 lg:w-20 opacity-0 animate-[carMoveEffect2_1s_forwards] [animation-delay:1.3s]"
                                  src={carGreen}
                                />
                              )}

                              {!place.isOccupied && !place.isReservedByMe && (
                                <div className="absolute flex items-center justify-center h-8 w-8 font-bold bg-blue-500 rounded-full">
                                  P
                                </div>
                              )}
                            </div>
                          ))}

                        </div>
                      )}
                    </div>
                  ))
                )}

              </div>
            </div>
          </div>
        ))}

      </div>
    </div>
  );
};

export default SelectPlace;