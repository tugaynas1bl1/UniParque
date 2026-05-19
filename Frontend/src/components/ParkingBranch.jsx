import React from "react";
import logo from "../assets/parkingLogo.png";
import { Link, useLocation } from "react-router-dom";

const ParkingBranch = ({
  branch,
  selected,
  toggleSelect,
  selectionMode,
}) => {

  const location = useLocation()

  return (
    <Link
      to={`/${!location.pathname.startsWith("/admin") ? "select-place" : "admin/manage-places"}/${branch.id}`}
      className="group relative cursor-pointer"
    >

      {selectionMode && (
        <input
          type="checkbox"
          checked={selected}
          onClick={(e) => e.stopPropagation()}
          onChange={toggleSelect}
          className="absolute top-2 left-2 w-5 h-5 z-10"
        />
      )}

      {/* GLOW */}
      <div className="absolute -inset-0.5 bg-gradient-to-r from-[#FC563C] to-orange-600 
                      rounded-2xl blur opacity-30 group-hover:opacity-100 transition duration-500">
      </div>

      {/* BG */}
      <img
        className="absolute z-1 opacity-10 blur-[15px] w-full h-full rounded-2xl"
        src={logo}
        alt=""
      />

      {/* CARD */}
      <div
        className="relative bg-[#11161D]/80 backdrop-blur-xl 
                   border border-white/10 
                   rounded-2xl p-6 h-44 
                   flex flex-col justify-center items-center
                   transition-all duration-500
                   group-hover:scale-105 group-hover:-translate-y-2"
      >
        <div
          className="w-14 h-14 flex items-center justify-center 
                     rounded-full bg-[#FC563C]/20 text-[#FC563C] 
                     text-xl font-bold mb-4
                     group-hover:rotate-6 transition"
        >
          P
        </div>

        <h2 className="text-lg font-semibold text-center">
          {branch.branchName}
        </h2>
      </div>
    </Link>
  );
};

export default ParkingBranch;