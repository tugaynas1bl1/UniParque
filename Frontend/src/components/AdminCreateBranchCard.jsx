import React, { useState } from "react";
import toast from "react-hot-toast";

const AdminCreateBranchCard = ({ onClose, onCreate }) => {
  const [name, setName] = useState("");
  const [loading, setLoading] = useState(false);

  const handleCreate = async () => {
    if (!name.trim()) return;

    try {
      setLoading(true);
      await onCreate(name);
      setName("");
      onClose();
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center"
    >
      <div
        onClick={onClose}
        className="absolute inset-0 bg-black/50 backdrop-blur-sm"
      />

      {/* MODAL */}
      <div
        onClick={(e) => e.stopPropagation()}
        className="relative z-10 w-[350px] p-6 rounded-2xl
                   bg-[#11161D]/90 backdrop-blur-xl
                   border border-white/10
                   shadow-xl"
      >
        <h2 className="text-xl font-semibold mb-4 text-center">
          Create Branch
        </h2>

        <input
          type="text"
          placeholder="Branch name..."
          value={name}
          onChange={(e) => setName(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              if (name.length < 3)
                toast.error("Branch name length should be greater than 2 letters")
              else
                handleCreate();
            } 
          }}
          className="w-full px-3 py-2 rounded-lg bg-gray-800 border border-gray-700 
                     outline-none focus:border-green-500 text-sm mb-4"
        />

        <div className="flex gap-3">
          <button
            onClick={onClose}
            className="flex-1 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 transition"
          >
            Cancel
          </button>

          <button
            onClick={(e) => {
              if (name.length < 3)
                toast.error("Branch name length should be greater than 2 letters")
              else
                handleCreate();
            }}
            disabled={loading || !name.trim()}
            className={`flex-1 py-2 rounded-lg transition ${
              loading || !name.trim()
                ? "bg-gray-700 cursor-not-allowed"
                : "bg-green-600 hover:bg-green-700"
            }`}
          >
            {loading ? "Creating..." : "Create"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminCreateBranchCard;