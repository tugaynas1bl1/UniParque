import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import api from '../utils/axios';
import { Trash2 } from 'lucide-react';
import AdminLayoutAddModule from '../components/AdminLayoutAddModule';
import ReservationModule from '../components/AdminReservationCreateCard';
import toast from 'react-hot-toast';
import car from '../assets/carRed.png';
import reserved from '../assets/reservedTable.png';
import LoadingWhite from '../components/LoadingWhite';

const AdminFullLayout = () => {
    const [fullLayout, setFullLayout] = useState(null);
    const { branchId } = useParams();

    const [isVisible, setIsVisible] = useState(false);
    const [placeholder, setPlaceholder] = useState("");
    const [moduleHeader, setModuleHeader] = useState("");
    const [selected, setSelected] = useState({ subSectionId: '', sectionId: '' });

    const [reservationVisible, setReservationVisible] = useState(false);
    const [selectedPlaceId, setSelectedPlaceId] = useState(null);

    const [isLoading, setIsLoading] = useState(false)

    const getSections = async () => {
        try {
            setIsLoading(true)

            const { data, statusText } = await api.get(`ParkingBranches/full-layout/${branchId}`);
            if (statusText === 'OK' && data) setFullLayout(data.data || null);
        } catch (error) { console.error(error); }
        finally {
            setIsLoading(false)
        }
    };

    const createReservation = async (data) => {
        try {
            const payload = {
                userId: "CURRENT_USER_ID",
                placeId: data.placeId,
                estimatedArrivalTime: data.estimatedArrivalTime,
                carNumber: data.carNumber
            };

            const res = await api.post("reservation/create-by-user-id", payload);

            if (res.data.success) {
                toast.success("Reserved successfully");
                setReservationVisible(false);
                getSections();
            } else {
                toast.error("Reservation failed");
            }
        } catch (err) {
            toast.error("Reservation error");
            console.error(err);
        }
    };

    const cancelReservation = async (placeId) => {
        try {
            const { data } = await api.delete(`reservation/${placeId}`);

            if (data.success) {
                toast.success("Reservation cancelled");
                getSections();
            } else {
                toast.error("Cancel failed");
            }
        } catch (err) {
            toast.error("Cancel error");
            console.error(err);
        }
    };

    const handleAddPlace = (subSectionId) => {
        setPlaceholder("Place name");
        setModuleHeader("Add a Place");
        setSelected({ subSectionId });
        setIsVisible(true);
    };

    const handleAddSection = () => {
        setPlaceholder("Section name");
        setModuleHeader("Add a Section");
        setSelected({});
        setIsVisible(true);
    };

    const handleAddSubSection = (sectionId) => {
        setPlaceholder("Subsection name");
        setModuleHeader("Add a SubSection");
        setSelected({ sectionId });
        setIsVisible(true);
    };

    const onDone = async (value) => {
        try {
            let payload = {};

            if (moduleHeader === "Add a Place") {
                payload = { subSectionId: selected.subSectionId, placeName: value };
                const { data } = await api.post('ParkingPlaces', payload);
                data.success ? toast.success("Place added successfully") : toast.error("Failed to add place!");
            }
            else if (moduleHeader === "Add a Section") {
                payload = { branchId, section: value };
                const { data } = await api.post('ParkingSections', payload);
                data.success ? toast.success("Section added successfully") : toast.error("Failed to add section!");
            }
            else if (moduleHeader === "Add a SubSection") {
                payload = { sectionId: selected.sectionId, subSection: value };
                const { data } = await api.post('ParkingSubSections', payload);
                data.success ? toast.success("SubSection added successfully") : toast.error("Failed to add subsection!");
            }

            setIsVisible(false);
            getSections();
        } catch (error) {
            toast.error("Operation failed!");
            console.error(error);
        }
    };

    const deletePlace = async (id) => {
        try {
            const { data } = await api.delete(`ParkingPlaces/${id}`);
            data.success ? toast.success("Place deleted") : toast.error("Delete failed");
            getSections();
        } catch (err) { toast.error("Delete error"); }
    };

    const deleteSubSection = async (id) => {
        try {
            const { data } = await api.delete(`ParkingSubSections/${id}`);
            data.success ? toast.success("SubSection deleted") : toast.error("Delete failed");
            getSections();
        } catch (err) { toast.error("Delete error"); }
    };

    const deleteSection = async (id) => {
        try {
            const { data } = await api.delete(`ParkingSections/${id}`);
            data.success ? toast.success("Section deleted") : toast.error("Delete failed");
            getSections();
        } catch (err) { toast.error("Delete error"); }
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

      const handlePlaceOccupied = ({ placeId, isOccupied }) => {
        setFullLayout(prev => {
          if (!prev) return prev;
          return {
            ...prev,
            sections: prev.sections.map(section => ({
              ...section,
              subSections: section.subSections.map(sub => ({
                ...sub,
                places: sub.places.map(p => p.id === placeId ? { ...p, isOccupied } : p),
              })),
            })),
          };
        });
      };
    
    useReservationSignalR(branchId, handlePlaceReserved);
    useReservationSignalR(branchId, handlePlaceOccupied);


    useEffect(() => { getSections(); }, [branchId]);

    if (!fullLayout) return <LoadingWhite />;

    return (
        <div className='flex flex-col gap-6 sm:gap-8 lg:gap-10'>

    {isVisible && (
        <AdminLayoutAddModule
            setIsVisible={setIsVisible}
            placeholder={placeholder}
            header={moduleHeader}
            onDone={onDone}
        />
    )}

    {reservationVisible && (
        <ReservationModule
            setIsVisible={setReservationVisible}
            placeId={selectedPlaceId}
            onSubmit={createReservation}
        />
    )}

    <h1 className='text-2xl sm:text-3xl lg:text-4xl font-bold text-center px-2 mt-12 lg:mt-0'>
        "{fullLayout.branchName}" Parking Layout
    </h1>

    <div className='text-center mb-5'>
        <button
            onClick={handleAddSection}
            className='px-4 sm:px-5 py-2 text-sm sm:text-base cursor-pointer bg-orange-500 shadow-md shadow-orange-200 text-white rounded-full hover:bg-orange-800 hover:scale-110 transition-all duration-200'
        >
            + Add Section
        </button>
    </div>

    {fullLayout.sections.length !== 0 ? (
        <div>
            {fullLayout.sections.map((section) => (
                <div
                    key={section.id}
                    className='flex flex-col gap-6 sm:gap-8 lg:gap-10 justify-center relative'
                >

                    <div className='flex justify-between items-center'>
                        <h1 className='text-[16px] sm:text-[18px] lg:text-[20px] font-semibold bg-orange-600 p-2 rounded-t-xl w-full text-center'>
                            Section {section.name}
                        </h1>

                        <Trash2
                            onClick={() => deleteSection(section.id)}
                            className='cursor-pointer hover:scale-125 text-red-600 w-5 h-5 sm:w-6 sm:h-6 m-2 transition-all'
                        />
                    </div>

                    <div className='flex justify-center'>
                        <button
                            onClick={() => handleAddSubSection(section.id)}
                            className='px-3 sm:px-4 py-2 text-sm sm:text-base cursor-pointer hover:scale-110 shadow-md shadow-green-200 bg-green-600 text-white rounded-full hover:bg-green-800 transition-all duration-200'
                        >
                            + Add SubSection
                        </button>
                    </div>

                    <div className='flex flex-col gap-6 sm:gap-8 lg:gap-10 justify-center bg-gray-900'>
                        {section.subSections.map((subSection) => (
                            <div key={subSection.id} className='flex flex-col flex-wrap justify-center items-center'>

                                <div className='flex justify-between items-center text-center text-lg sm:text-xl lg:text-2xl font-bold p-3'>
                                    <h3 className='text-center w-full break-words'>
                                        {subSection.name}
                                    </h3>

                                    

                                    <Trash2
                                        onClick={() => deleteSubSection(subSection.id)}
                                        className='cursor-pointer hover:scale-125 text-red-600 w-4 h-4 sm:w-5 sm:h-5 transition-all'
                                    />
                                </div>

                                <div
                                        onClick={() => handleAddPlace(subSection.id)}
                                        className='ml-0 mt-10 lg:ml-5 w-12 sm:w-14 lg:w-10 h-10 cursor-pointer hover:scale-110 text-xl sm:text-2xl shadow-md shadow-blue-400 bg-blue-600/40 hover:bg-blue-950/50 font-semibold rounded-full flex justify-center items-center transition-all duration-200'
                                    >
                                        <p>+</p>
                                </div>

                                <div className='flex flex-wrap gap-4 sm:gap-6 lg:gap-10 mb-10 mt-10 justify-center lg:justify-start'>

                                    

                                    {subSection.places.map((place) => (
                                        <div
                                            key={place.id}
                                            onClick={() => {
                                                if (!place.isOccupied && !place.isReserved) {
                                                    setSelectedPlaceId(place.id);
                                                    setReservationVisible(true);
                                                } else if (place.isReserved) {
                                                    cancelReservation(place.id);
                                                }
                                            }}
                                            className={`w-16 sm:w-18 lg:w-20 h-24 sm:h-28 lg:h-30 border-x-4 border-white relative flex flex-col justify-center items-center transition-all duration-300 cursor-pointer
                                                ${place.isOccupied
                                                    ? 'bg-red-700 shadow-md shadow-red-500/40'
                                                    : place.isReserved
                                                        ? 'bg-yellow-500 shadow-md shadow-yellow-300/40'
                                                        : 'bg-gray-700 hover:scale-110'}`}
                                        >

                                            <Trash2
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    deletePlace(place.id);
                                                }}
                                                className='absolute -top-2 -right-2 sm:-right-3 hover:scale-140 hover:bg-white hover:text-red-600 bg-red-600 w-4 h-4 sm:w-5 sm:h-5 p-0.5 rounded-md cursor-pointer transition-all duration-300'
                                            />

                                            <div className='mx-auto h-full border-l-4 border-dashed border-white w-1 relative'></div>

                                            <p
                                                className={`mx-auto text-center absolute text-lg sm:text-xl lg:text-2xl flex justify-center items-center p-1 w-8 h-8 sm:w-9 sm:h-9 lg:w-10 lg:h-10 rounded-[50%] font-semibold
                                                ${place.isOccupied || place.isReserved ? 'hidden' : 'bg-blue-500 text-white'}`}
                                            >
                                                P
                                            </p>

                                            {place.isOccupied && (
                                                <img
                                                    className='absolute scale-110 sm:scale-125 lg:scale-150 animate-[carMoveEffect2_1s]'
                                                    src={car}
                                                    alt=""
                                                />
                                            )}

                                            {place.isReserved && (
                                                <img
                                                    className='absolute scale-110 sm:scale-120 lg:scale-130 animate-[stampEffect_0.3s]'
                                                    src={reserved}
                                                    alt=""
                                                />
                                            )}
                                        </div>
                                    ))}
                                </div>

                                <div className='bg-gray-500 w-full h-[0.1px]'></div>
                            </div>
                        ))}
                    </div>
                </div>
            ))}
        </div>
    ) : (
        <p className="text-gray-500 text-center mt-20">
            This branch is empty!
        </p>
    )}

</div>
    );
};

export default AdminFullLayout;