import React, { useEffect, useState } from "react";
import { Trash2 , Plus} from "lucide-react";
import api from "../utils/axios";
import toast from "react-hot-toast";
import AdminCreateBranchCard from "../components/AdminCreateBranchCard";
import ParkingBranch from "../components/ParkingBranch";
import LoadingWhite from "../components/LoadingWhite";

export default function AdminParkingBranches() {
  const [branches, setBranches] = useState([]);
  const [filteredBranches, setFilteredBranches] = useState([]);
  const [search, setSearch] = useState('');
  const [selected, setSelected] = useState([]);
  const [selectionMode, setSelectionMode] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getBranches = async () => {
    try {
      setIsLoading(true)
      const { data } = await api.get("ParkingBranches/all");
      if (data.success) {
        setBranches(data.data);
        setFilteredBranches(data.data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false)
    }
  };

  useEffect(() => {
    getBranches();
  }, []);

  useEffect(() => {
    const filtered = branches.filter(b =>
      (b.branchName || '').toLowerCase().includes(search.toLowerCase())
    );
    setFilteredBranches(filtered);
  }, [search, branches]);

  const toggleSelect = (id) => {
    setSelected((prev) =>
      prev.includes(id)
        ? prev.filter((i) => i !== id)
        : [...prev, id]
    );
  };

  const toggleSelectAll = () => {
    if (selected.length === branches.length) {
      setSelected([]);
    } else {
      setSelected(branches.map((b) => b.id));
    }
  };

  const handleDelete = async () => {
    try {
      for (let id of selected) {
        await api.delete(`/ParkingBranches/${id}`);
      }
      toast.success("Selected branches deleted!");
      setSelected([]);
      setSelectionMode(false);
      getBranches();
    } catch (err) {
      toast.error("Something went wrong");
      console.error(err);
    }
  };

  const handleCreate = async (name) => {
    try {
      const { data } = await api.post("/ParkingBranches", { branchName: name });
      if (data.success) {
        toast.success("Branch created!");
        getBranches();
      }
    } catch (err) {
      toast.error("Failed to create branch");
      console.error(err);
    }
  };

  const handleEdit = async (branch) => {
    const name = prompt("Edit branch name:", branch.branchName);
    if (!name || name === branch.branchName) return;

    try {
      const { data } = await api.put(`/branches/${branch.id}`, { branchName: name });
      if (data.success) {
        toast.success("Branch updated!");
        getBranches();
      }
    } catch (err) {
      toast.error("Failed to update branch");
      console.error(err);
    }
  };

  return (
    <div className="p-6 min-h-screen bg-gray-950 text-white flex flex-col items-center">

      {isLoading ? (
        <LoadingWhite />
      ) : (

      <> 

      <div className="w-full max-w-6xl flex flex-col md:flex-row justify-between items-center mb-6 gap-4">
        <h1 className="font-bold text-2xl mt-7 lg:text-3xl">Parking Branches</h1>

        <input
          type="text"
          placeholder="Search branches..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="px-4 py-2 rounded-4xl border-2 border-[#FC563C] text-white w-full max-w-sm focus:outline-none focus:ring-2 focus:ring-[#FC563C]"
        />

        <div className="flex gap-4">
          {!selectionMode && (
            <button
              onClick={() => { setSelectionMode(true); setSelected([]); }}
              className="px-4 py-2 bg-gray-800 rounded-xl hover:bg-gray-700 transition cursor-pointer"
            >
              Select
            </button>
          )}

          {selectionMode && (
            <>
              <button
                onClick={toggleSelectAll}
                className="px-4 py-2 bg-gray-800 rounded-xl hover:bg-gray-700 transition cursor-pointer"
              >
                {selected.length === branches.length ? "Unselect All" : "Select All"}
              </button>

              <button
                onClick={handleDelete}
                disabled={selected.length === 0}
                className={`px-4 py-2 rounded-xl transition ${
                  selected.length === 0
                    ? "bg-gray-700/50 cursor-not-allowed"
                    : "bg-red-600 hover:bg-red-700 cursor-pointer"
                }`}
              >
                <Trash2 size={16} className="inline mr-1" />
                Delete
              </button>

              <button
                onClick={() => { setSelectionMode(false); setSelected([]); }}
                className="px-4 py-2 bg-gray-700 rounded-xl hover:bg-gray-600 transition cursor-pointer"
              >
                Cancel
              </button>
            </>
          )}
        </div>
      </div>

      <div className="grid md:grid-cols-3 gap-6 w-full">
        {search === '' && <div
          onClick={() => setShowCreateModal(true)}
          className="relative group cursor-pointer
                    rounded-2xl border border-white/10 
                    bg-[#11161D]/80 backdrop-blur-xl
                    h-44 flex flex-col justify-center items-center
                    transition-transform duration-500 hover:scale-105 hover:-translate-y-2 "
        >
          <div className="absolute -inset-0.5 bg-gradient-to-r from-green-400 to-teal-500
                          rounded-2xl blur opacity-30 group-hover:opacity-100 transition duration-500 "></div>

          <div className="w-16 h-16 flex items-center justify-center rounded-full 
                          bg-green-600/20 text-green-400 text-3xl font-bold mb-4
                          group-hover:rotate-12 transition group-hover:text-white">
            <Plus size={28} />
          </div>

          <h2 className="text-lg font-semibold text-center text-green-400">
            Create Branch
          </h2>
        </div> }

        {showCreateModal && (
          <AdminCreateBranchCard
            onClose={() => setShowCreateModal(false)}
            onCreate={handleCreate}
          />
        )}

        {filteredBranches.length > 0 ? (
          filteredBranches.map((branch) => (
            <ParkingBranch
              key={branch.id}
              branch={branch}
              selected={selected.includes(branch.id)}
              toggleSelect={() => toggleSelect(branch.id)}
              selectionMode={selectionMode}
              onEdit={() => handleEdit(branch)}
            />
          ))
        ) : (
          <p className="text-center text-gray-400 col-span-full mt-10">
            No branches found
          </p>
        )}
      </div>
      </>
      )}
    </div>
  );
}