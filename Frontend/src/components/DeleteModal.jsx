import React from "react";

export default function DeleteModal({ 
  title = "Delete Item", 
  message = "Are you sure you want to delete this?", 
  onCancel, 
  onConfirm, 
  loading = false 
}) {
  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-gray-900 p-6 rounded-2xl shadow-xl w-[350px] text-center">
        <h2 className="text-xl font-semibold text-white mb-4">{title}</h2>
        <p className="text-gray-400 mb-6">{message}</p>

        <div className="flex justify-center gap-4">
          <button
            onClick={onCancel}
            className="px-4 text-white py-2 rounded-xl bg-gray-700 hover:bg-gray-600 transition"
          >
            Cancel
          </button>

          <button
            onClick={onConfirm}
            disabled={loading}
            className="px-4 py-2 text-white rounded-xl bg-red-600 hover:bg-red-700 transition disabled:opacity-50"
          >
            {loading ? "Deleting..." : "Delete"}
          </button>
        </div>
      </div>
    </div>
  );
}