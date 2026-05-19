import React, { useEffect, useState } from "react";
import api from "../utils/axios";
import { useTranslation } from "react-i18next";

const PastReservations = () => {
  const { t } = useTranslation();
  const [reservations, setReservations] = useState({
    items: [],
    hasNext: false,
    hasPrevious: false,
  });
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);

  const statuses = ["completed", "cancelled", "expired"];

  const fetchReservations = async () => {
    try {
      setLoading(true);

      const responses = await Promise.all(
        statuses.map((status) =>
          api.get(`Reservations?Status=${status}&Page=${page}`)
        )
      );

      let allItems = [];

      responses.forEach((res) => {
        if (res.data?.data?.items) {
          allItems = [...allItems, ...res.data.data.items];
        }
      });

      setReservations({
        items: allItems,
        hasNext: responses[0].data.data.hasNext,
        hasPrevious: responses[0].data.data.hasPrevious,
      });
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReservations();
  }, [page]);

  const getStatusColor = (status) => {
    switch (status.toLowerCase()) {
      case "completed":
        return "bg-green-600";
      case "cancelled":
        return "bg-red-600";
      case "expired":
        return "bg-yellow-600";
      default:
        return "bg-gray-600";
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6 mt-11">
      <h1 className="text-3xl font-bold mb-6 text-center mt-10 mb-10">
        {t("pastReservations.title")}
      </h1>

      {loading ? (
        <div className="text-center text-gray-400">{t("pastReservations.loading")}</div>
      ) : reservations.items.length === 0 ? (
        <div className="text-center text-gray-500">{t("pastReservations.noReservations")}</div>
      ) : (
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {reservations.items.map((res) => (
            <div
              key={res.id}
              className="bg-gray-800 p-5 rounded-2xl shadow-lg hover:shadow-xl transition duration-300"
            >
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-lg font-semibold">{res.place}</h2>
                <span
                  className={`text-xs px-3 py-1 rounded-full ${getStatusColor(res.status)}`}
                >
                  {t(`pastReservations.status.${res.status.toLowerCase()}`)}
                </span>
              </div>

              <div className="space-y-2 text-gray-300 text-sm">
                <p>
                  <span className="text-gray-500">{t("pastReservations.branch")}:</span> {res.branch}
                </p>
                <p>
                  <span className="text-gray-500">{t("pastReservations.section")}:</span> {res.section}
                </p>
                <p>
                  <span className="text-gray-500">{t("pastReservations.subSection")}:</span> {res.subSection}
                </p>
                <p>
                  <span className="text-gray-500">{t("pastReservations.car")}:</span> {res.carNumber}
                </p>
              </div>

              <div className="mt-4 flex justify-between items-center">
                <span className="text-lg font-bold text-indigo-400">
                  {res.price} ₼
                </span>
              </div>

              <p className="text-sm text-gray-400 mt-2 justify-self-end">
                {formatDate(res.createdAt)}
              </p>
            </div>
          ))}
        </div>
      )}

      <div className="flex justify-between items-center mt-6">
        <button
          onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
          disabled={!reservations.hasPrevious}
          className={`${reservations.hasPrevious ? "" : "hidden"} cursor-pointer px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600`}
        >
          {t("pastReservations.previous")}
        </button>

        <span className="text-gray-400">{t("pastReservations.page")} {page}</span>

        <button
          onClick={() => setPage((prev) => prev + 1)}
          disabled={!reservations.hasNext}
          className={`${reservations.hasNext ? "" : "hidden"} cursor-pointer px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600`}
        >
          {t("pastReservations.next")}
        </button>
      </div>
    </div>
  );
};

export default PastReservations;