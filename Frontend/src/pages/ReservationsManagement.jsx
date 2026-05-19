import React, { useEffect, useState } from "react";
import { Trash2 } from "lucide-react";
import api from "../utils/axios";
import toast from "react-hot-toast";
import moment from "moment";
import DeleteModal from "../components/DeleteModal";
import LoadingWhite from "../components/LoadingWhite";

const ReservationsManagement = () => {
  const [reservations, setReservations] = useState([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedId, setSelectedId] = useState(null);
  const [loadingDelete, setLoadingDelete] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getReservations = async () => {
    try {
      setIsLoading(true)
      const { data } = await api.get("/reservations/all");
      if (data.success) setReservations(data.data);
    } catch (error) {
      console.error(error);
      setReservations([]);
    } finally {
      setIsLoading(false)
    }
  };

  useEffect(() => {
    getReservations();
  }, []);

  const openDeleteModal = (id) => {
    setSelectedId(id);
    setModalOpen(true);
  };

  const handleDelete = async () => {
    if (!selectedId) return;
    setLoadingDelete(true);
    try {
      const { data } = await api.delete(`/reservations/${selectedId}`);
      if (data.success) {
        toast.success("Reservation deleted successfully");
        setReservations(reservations.filter((r) => r.id !== selectedId));
        setModalOpen(false);
      }
    } catch (error) {
      toast.error("Something went wrong");
      console.error(error);
    } finally {
      setLoadingDelete(false);
    }
  };

  return (
    <div className="p-4 sm:p-6 min-h-screen bg-gray-950 text-white flex flex-col items-center">

      {isLoading ? (
        <LoadingWhite />
      ) : (

      <> 

      <h1 className="text-2xl sm:text-3xl font-bold mb-6 mt-7 text-center">
        Reservations Management
      </h1>

      {reservations.length === 0 && (
        <p className="text-center text-gray-400 text-base sm:text-lg">
          No reservations
        </p>
      )}

      {/* GRID FIXED */}
      <div className="w-full max-w-7xl grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5 sm:gap-6">
        {reservations.map((res) => (
          <div
            key={res.id}
            className="bg-gray-900 p-4 sm:p-5 rounded-2xl shadow-lg hover:shadow-xl transition flex flex-col gap-3 relative"
          >
            <div className="flex flex-col gap-2">
              <h3 className="font-semibold text-base sm:text-lg">
                {res.user}
              </h3>

              <div className="grid grid-cols-2 gap-2 text-gray-400 text-xs sm:text-sm">
                <div>
                  <span className="font-semibold text-gray-300">Branch</span>
                  <p>{res.branch}</p>
                </div>

                <div>
                  <span className="font-semibold text-gray-300">Section</span>
                  <p>{res.section}</p>
                </div>

                <div>
                  <span className="font-semibold text-gray-300">Subsection</span>
                  <p>{res.subSection}</p>
                </div>

                <div>
                  <span className="font-semibold text-gray-300">Place</span>
                  <p>{res.place}</p>
                </div>

                <div>
                  <span className="font-semibold text-gray-300">Car Number</span>
                  <p>{res.carNumber}</p>
                </div>

                <div>
                  <span className="font-semibold text-gray-300">Price</span>
                  <p>${res.price}</p>
                </div>
              </div>

              <div className="text-gray-500 text-xs mt-2">
                {moment(new Date(res.createdAt)).format("DD MMMM YYYY, HH:mm")}
              </div>
            </div>

            {/* STATUS FIXED (NO ABSOLUTE) */}
            <div className="mt-2">
              <span
                className={`inline-block px-3 py-1 rounded-full text-xs sm:text-sm ${
                  res.status === "Active" || res.status === "Completed"
                    ? "bg-green-600"
                    : res.status === "Expired" || res.status === "Cancelled"
                    ? "bg-red-600"
                    : "bg-blue-600"
                }`}
              >
                {res.status}
              </span>
            </div>

            {/* DELETE BUTTON */}
            <div className="flex justify-end mt-2">
              <button
                onClick={() => openDeleteModal(res.id)}
                className="p-2 rounded-xl bg-red-600 hover:bg-red-700 transition"
              >
                <Trash2 size={16} />
              </button>
            </div>
          </div>
        ))}
      </div>

      {modalOpen && (
        <DeleteModal
          title="Delete Reservation"
          message="Are you sure you want to delete this reservation?"
          onCancel={() => setModalOpen(false)}
          onConfirm={handleDelete}
          loading={loadingDelete}
        />
      )}
      </>
      )}
    </div>
  );
};

export default ReservationsManagement;