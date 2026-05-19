import React, { useEffect, useState } from "react";
import { Search, Trash2 } from "lucide-react";
import api from "../utils/axios";
import toast from "react-hot-toast";
import defaultUserImg from "../assets/user.png";
import { useNavigate } from "react-router-dom";
import DeleteModal from "../components/DeleteModal";
import LoadingWhite from "../components/LoadingWhite";


export default function UsersManagement() {
  const [users, setUsers] = useState([]);
  const [search, setSearch] = useState("");
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [loadingDelete, setLoadingDelete] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  const getUsers = async () => {
    try {
      setIsLoading(true);
      const { data } = await api.get("auth/all-users");
      if (data.success) setUsers(data.data);
    } catch (error) {
      console.error(error);
      setUsers([]);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    getUsers();
  }, []);

  const handleDelete = async (id) => {
    try {
      setLoadingDelete(true);
      const { data } = await api.delete(`auth/user/${id}`);
      if (data.success) {
        toast.success("User deleted successfully");
        setUsers(users.filter((u) => u.id !== id));
        setShowDeleteModal(false);
        setSelectedUser(null);
      }
    } catch (error) {
      toast.error("Something went wrong");
    } finally {
      setLoadingDelete(false);
    }
  };

  const goToUserProfile = (userId) => {
    navigate(`/admin/users/${userId}`);
  };

  const filteredUsers = (users || []).filter(
    (u) =>
      `${u.firstName} ${u.lastName}`.toLowerCase().includes(search.toLowerCase()) ||
      u.email.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="min-h-screen w-full overflow-x-hidden bg-gray-950 text-white p-4 sm:p-6 md:p-8 flex flex-col items-center">

      {isLoading ? (
        <LoadingWhite />
      ) : (
        <>

          <h1 className="text-2xl sm:text-3xl font-bold mt-7 mb-6 text-center">
            Users Management
          </h1>


          <div className="flex items-center bg-gray-900 rounded-2xl px-4 py-2 mb-6 w-full sm:w-2/3 md:w-1/3">
            <Search className="w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search users..."
              className="bg-transparent outline-none px-2 w-full text-sm sm:text-base"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>


          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 w-full max-w-6xl mt-10">
            {filteredUsers.length > 0 ? (
              filteredUsers.map((user) =>
                user.role !== "Admin" ? (
                  <div
                    key={user.id}
                    className="relative p-4 pt-16 rounded-2xl bg-gray-800 flex flex-col items-center hover:scale-105 transition cursor-pointer"
                  >
                    <img
                      className="absolute -top-8 w-20 h-20 rounded-full border-4 border-gray-950 object-cover"
                      src={user.photo || defaultUserImg}
                      alt={user.firstName}
                    />

                    <div
                      className="w-full text-center"
                      onClick={() => goToUserProfile(user.id)}
                    >
                      <p className="text-sm">
                        <span className="font-bold text-gray-300">Email:</span>{" "}
                        {user.email}
                      </p>
                      <p className="text-sm">
                        <span className="font-bold text-gray-300">Firstname:</span>{" "}
                        {user.firstName}
                      </p>
                      <p className="text-sm">
                        <span className="font-bold text-gray-300">Lastname:</span>{" "}
                        {user.lastName}
                      </p>
                    </div>

                    <div className="flex justify-between items-center w-full mt-4 px-2">
                      <span className="px-3 py-1 text-xs rounded-full bg-blue-600">
                        {user.role}
                      </span>

                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          setSelectedUser(user);
                          setShowDeleteModal(true);
                        }}
                        className="p-2 rounded-xl bg-red-600 hover:bg-red-700 transition"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </div>
                ) : null
              )
            ) : (
              <p className="text-center text-gray-400 col-span-full">
                No users found
              </p>
            )}
          </div>

          {showDeleteModal && selectedUser && (
            <DeleteModal
              title="Delete User"
              message={`Are you sure you want to delete ${selectedUser.firstName} ${selectedUser.lastName}?`}
              onCancel={() => setShowDeleteModal(false)}
              onConfirm={() => handleDelete(selectedUser.id)}
              loading={loadingDelete}
            />
          )}
        </>
      )}
    </div>
  );
}