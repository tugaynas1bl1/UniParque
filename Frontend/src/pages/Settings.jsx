import React, { useEffect, useState } from "react";
import { useTokens } from "../stores/tokenStore";
import { useNavigate } from "react-router-dom";
import api from "../utils/axios";
import toast from "react-hot-toast";
import { Key } from "lucide-react";
import LoadingWhite from "../components/LoadingWhite";

const Settings = () => {
  const { accessToken, clearTokens } = useTokens();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);
  const [isPageLoading, setIsPageLoading] = useState(false);

  const [userData, setUserData] = useState({
    firstName: "",
    lastName: "",
    email: ""
  });

  const [savedUserData, setSavedUserData] = useState({
    firstName: "",
    lastName: "",
    email: ""
  });

  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [isBtnDisabled, setIsBtnDisabled] = useState(true);

  const getAdminInfo = async () => {
    try {
      setIsPageLoading(true)

      const { data } = await api.get("auth/my-info");

      if (data.success) {
        const info = {
          firstName: data.data.firstName,
          lastName: data.data.lastName,
          email: data.data.email
        };

        setUserData(info);
        setSavedUserData(info);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setIsPageLoading(false)
    }
  };

  const updateAdminInfo = async () => {
    try {
      setLoading(true);

      const { data } = await api.patch("auth/edit-profile", {
        firstName: userData.firstName,
        lastName: userData.lastName
      });

      if (data.success) {
        toast.success("Profile updated");
        getAdminInfo();
      }
    } catch (err) {
      console.error(err);
      toast.error("Update failed");
    } finally {
      setLoading(false);
    }
  };

  const handleChangePassword = async () => {
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/;

    if (newPassword !== confirmPassword) {
      toast.error("Passwords do not match");
      return;
    }

    if (newPassword.length < 8) {
      toast.error("Password must be at least 8 characters");
      return;
    }

    if (!passwordRegex.test(newPassword)) {
      toast.error(
        "Password must contain at least 1 uppercase, 1 lowercase and 1 number"
      );
      return;
    }

    try {
      setLoading(true);

      await api.put("auth/change-password", {
        oldPassword,
        newPassword,
        confirmNewPassword: confirmPassword
      });

      toast.success("Password updated");

      setOldPassword("");
      setNewPassword("");
      setConfirmPassword("");
    } catch (err) {
      toast.error("Password change failed");
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    clearTokens();
    navigate("/");
  };

  useEffect(() => {
    const changed =
      userData.firstName !== savedUserData.firstName ||
      userData.lastName !== savedUserData.lastName;

    setIsBtnDisabled(!changed);
  }, [userData]);

  const isPasswordValid =
    oldPassword &&
    newPassword &&
    confirmPassword &&
    newPassword === confirmPassword;

  useEffect(() => {
    if (!accessToken) navigate("/");
    getAdminInfo();
  }, [accessToken]);

  const handleInput = (key, value) => {
    setUserData((prev) => ({ ...prev, [key]: value }));
  };

  document.body.style.overflow = "auto";

  return (
    <div className="min-h-screen bg-gray-950 text-white p-4 sm:p-6 md:p-8">

      {isPageLoading ? (<LoadingWhite />) : (

        <>

      <div className="max-w-2xl mx-auto space-y-6">

        <h2 className="text-2xl sm:text-3xl font-bold text-center mt-7 sm:text-left">
          Admin Settings
        </h2>

        {/* PROFILE */}
        <div className="bg-[#11161D] p-4 sm:p-6 rounded-2xl border border-white/10 shadow-lg">
          <h3 className="text-base sm:text-lg font-semibold mb-4">
            Profile Information
          </h3>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input
              value={userData.firstName}
              onChange={(e) => handleInput("firstName", e.target.value)}
              placeholder="First Name"
              className="bg-gray-900 border border-white/10 px-4 py-2 rounded-lg outline-none focus:border-[#FC563C]"
            />

            <input
              value={userData.lastName}
              onChange={(e) => handleInput("lastName", e.target.value)}
              placeholder="Last Name"
              className="bg-gray-900 border border-white/10 px-4 py-2 rounded-lg outline-none focus:border-[#FC563C]"
            />
          </div>

          <div className="mt-4">
            <label className="text-gray-400 text-xs sm:text-sm">Email</label>
            <input
              value={userData.email}
              disabled
              className="w-full mt-1 bg-gray-800 text-gray-400 px-4 py-2 rounded-lg text-sm sm:text-base"
            />
          </div>

          <button
            disabled={isBtnDisabled || loading}
            onClick={updateAdminInfo}
            className={`mt-6 w-full py-2 rounded-lg font-semibold transition text-sm sm:text-base ${
              isBtnDisabled
                ? "bg-gray-700 cursor-not-allowed"
                : "bg-[#FC563C] hover:bg-[#e14b33]"
            }`}
          >
            Save Changes
          </button>
        </div>

        {/* PASSWORD */}
        <div className="bg-[#11161D] p-4 sm:p-6 rounded-2xl border border-white/10 shadow-lg">
          <h3 className="text-base sm:text-lg font-semibold mb-4 flex items-center gap-2">
            <Key size={18} /> Change Password
          </h3>

          <div className="space-y-3 sm:space-y-4">
            <input
              type="password"
              placeholder="Current Password"
              value={oldPassword}
              onChange={(e) => setOldPassword(e.target.value)}
              className="w-full bg-gray-900 px-4 py-2 rounded-lg outline-none text-sm sm:text-base"
            />

            <input
              type="password"
              placeholder="New Password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="w-full bg-gray-900 px-4 py-2 rounded-lg outline-none text-sm sm:text-base"
            />

            <input
              type="password"
              placeholder="Confirm New Password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="w-full bg-gray-900 px-4 py-2 rounded-lg outline-none text-sm sm:text-base"
            />

            <button
              disabled={!isPasswordValid || loading}
              onClick={handleChangePassword}
              className={`w-full py-2 rounded-lg font-semibold transition text-sm sm:text-base ${
                isPasswordValid
                  ? "bg-[#FC563C] hover:bg-[#e14b33]"
                  : "bg-gray-700 cursor-not-allowed"
              }`}
            >
              Update Password
            </button>
          </div>
        </div>

        {/* LOGOUT */}
        <div className="bg-[#11161D] p-4 sm:p-6 rounded-2xl border border-white/10 shadow-lg">
          <button
            onClick={handleLogout}
            className="w-full bg-red-600 hover:bg-red-500 py-2 rounded-lg font-semibold transition text-sm sm:text-base"
          >
            Log Out
          </button>
        </div>

      </div>
      </>
      )}
    </div>
  );
};

export default Settings;