import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Trash2 } from "lucide-react";
import api from "../utils/axios";
import defaultUserImg from "../assets/user.png";
import toast from "react-hot-toast";
import DeleteModal from "../components/DeleteModal";

export default function AdminUserProfile() {
  const { id } = useParams();

  const [user, setUser] = useState(null);
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const [selectedReservation, setSelectedReservation] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [isVisible, setIsVisible] = useState(false);

  const formatDate = (date) => {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = d.getFullYear();
    return `${day}.${month}.${year}`;
  };

  const formatDateTime = (date) => {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = d.getFullYear();
    const hours = String(d.getHours()).padStart(2, "0");
    const minutes = String(d.getMinutes()).padStart(2, "0");
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  const getUser = async () => {
    try {
      const { data } = await api.get(`auth/user/${id}`);
      if (data.success) setUser(data.data);
    } catch (err) {
      console.error(err);
    }
  };

  const getReservations = async (pageNumber = 1) => {
    try {
      setLoading(true);
      const { data } = await api.get(
        `Reservations?Page=${pageNumber}&UserId=${id}`
      );
      if (data.success) {
        setReservations(data.data.items);
        setTotalPages(data.totalPages || 1);
      }
    } catch (err) {
      console.error(err);
      setReservations([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getUser();
    getReservations(page);
  }, [id, page]);

  const handleDeleteReservation = async () => {
    try {
      setDeleting(true);

      const { data } = await api.delete(
        `reservations/${selectedReservation.id}`
      );

      if (data.success) {
        setReservations((prev) =>
          prev.filter((r) => r.id !== selectedReservation.id)
        );

        toast.success("Reservation deleted");
        setShowDeleteModal(false);
        setSelectedReservation(null);
      }
    } catch (err) {
      console.error(err);
      toast.error("Delete failed");
    } finally {
      setDeleting(false);
    }
  };

  if (loading || !user) {
    return (
      <div className="text-white p-6 text-center text-lg">
        Loading...
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-950 text-white">

      {/* HEADER */}
      <div className="w-full h-48 sm:h-56 md:h-64 bg-gradient-to-r from-indigo-600 via-purple-600 to-pink-600 relative">
        <img
          src={user.photo || defaultUserImg}
          alt=""
          className="absolute left-1/2 sm:left-10 -translate-x-1/2 sm:translate-x-0 -bottom-12 sm:-bottom-16 w-24 h-24 sm:w-32 sm:h-32 md:w-36 md:h-36 rounded-full border-4 border-gray-950 object-cover"
        />
      </div>

      {/* USER INFO */}
      <div className="mt-16 sm:mt-20 md:mt-24 px-4 sm:px-6 md:px-10 text-center sm:text-left">
        <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold">
          {user.firstName} {user.lastName}
        </h2>
        <p className="text-gray-400 mt-2 text-sm sm:text-base md:text-lg">
          {user.email}
        </p>
        <span className="inline-block mt-3 sm:mt-4 px-4 sm:px-5 py-1 rounded-full bg-blue-600 text-xs sm:text-sm">
          {user.role}
        </span>
      </div>

      {/* RESERVATIONS */}
      <div className="mt-8 sm:mt-10 md:mt-12 px-4 sm:px-6 md:px-10 pb-10">
        <h3
          onClick={() => setIsVisible(!isVisible)}
          className="cursor-pointer text-lg sm:text-xl md:text-2xl font-semibold mb-4 sm:mb-6"
        >
          Reservations {isVisible ? "▲" : "▼"}
        </h3>

        {!isVisible && (
          <p className="text-gray-500 italic text-sm sm:text-base">
            Click to view reservations
          </p>
        )}

        {isVisible && (
          <>
            {reservations.length === 0 ? (
              <p className="text-gray-500 italic text-sm sm:text-base">
                This user hasn’t made any reservations yet.
              </p>
            ) : (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
                {reservations.map((r) => (
                  <div
                    key={r.id}
                    className="bg-gray-800 p-4 sm:p-5 rounded-2xl shadow hover:scale-[1.02] transition border border-gray-700"
                  >
                    <p className="text-sm sm:text-base"><span className="font-bold">Car:</span> {r.carNumber}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Branch:</span> {r.branch}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Section:</span> {r.section} / {r.subSection}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Place:</span> {r.place}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Arrival:</span> {formatDateTime(r.estimatedArrivalTime)}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Created:</span> {formatDate(r.createdAt)}</p>
                    <p className="text-sm sm:text-base"><span className="font-bold">Price:</span> {r.price} ₼</p>

                    <span
                      className={`mt-3 inline-block px-3 py-1 rounded-full text-xs sm:text-sm ${
                        r.status === "Active"
                          ? "bg-green-600"
                          : r.status === "Completed"
                          ? "bg-blue-600"
                          : r.status === "CheckedIn"
                          ? "bg-indigo-600"
                          : r.status === "Expired"
                          ? "bg-gray-500"
                          : r.status === "Cancelled"
                          ? "bg-red-600"
                          : "bg-gray-700"
                      }`}
                    >
                      {r.status}
                    </span>

                    <div className="flex justify-end mt-3 sm:mt-4">
                      <button
                        onClick={() => {
                          setSelectedReservation(r);
                          setShowDeleteModal(true);
                        }}
                        className="p-2 rounded-lg bg-red-600 hover:bg-red-700 transition"
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}

            {/* PAGINATION */}
            <div className="flex flex-col sm:flex-row justify-center items-center gap-3 sm:gap-4 mt-6">
              <button
                onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
                disabled={page === 1}
                className="px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600 disabled:opacity-50 transition w-full sm:w-auto"
              >
                Prev
              </button>

              <span className="px-3 py-2 bg-gray-800 rounded-lg text-sm sm:text-base">
                {page} / {totalPages}
              </span>

              <button
                onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
                disabled={page === totalPages}
                className="px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600 disabled:opacity-50 transition w-full sm:w-auto"
              >
                Next
              </button>
            </div>
          </>
        )}
      </div>

      {showDeleteModal && (
        <DeleteModal
          title="Delete Reservation"
          message="Are you sure you want to delete this reservation?"
          loading={deleting}
          onCancel={() => {
            setShowDeleteModal(false);
            setSelectedReservation(null);
          }}
          onConfirm={handleDeleteReservation}
        />
      )}
    </div>
  );
}