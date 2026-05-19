import React, { useState } from 'react'
import { FaUsers, FaCar, FaChartBar, FaCog, FaParking } from "react-icons/fa";
import { Link } from 'react-router-dom';

const AdminSideMenu = () => {
  const [isOpen, setIsOpen] = useState(true);
  const [mobileOpen, setMobileOpen] = useState(false);

  const toggleMenu = () => setIsOpen(prev => !prev);
  const toggleMobile = () => setMobileOpen(prev => !prev);

  const menuItems = [
    { name: "Dashboard", icon: <FaChartBar className="w-5 h-5 flex-shrink-0" />, to: "/admin/dashboard" },
    { name: "Users", icon: <FaUsers className="w-5 h-5 flex-shrink-0" />, to: "/admin/manage-users" },
    { name: "Branches", icon: <FaParking className="w-5 h-5 flex-shrink-0" />, to: "/admin/manage-branches" },
    { name: "Reservations", icon: <FaCar className="w-5 h-5 flex-shrink-0" />, to: "/admin/manage-reservations" },
    { name: "Settings", icon: <FaCog className="w-5 h-5 flex-shrink-0" />, to: "/admin/settings" },
  ];

  const HamburgerIcon = () => (
    <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
      <rect y="4" width="22" height="2" rx="1" fill="#9CA3AF"/>
      <rect y="10" width="22" height="2" rx="1" fill="#9CA3AF"/>
      <rect y="16" width="22" height="2" rx="1" fill="#9CA3AF"/>
    </svg>
  );

  const CloseIcon = () => (
    <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
      <line x1="3" y1="3" x2="19" y2="19" stroke="#9CA3AF" strokeWidth="2" strokeLinecap="round"/>
      <line x1="19" y1="3" x2="3" y2="19" stroke="#9CA3AF" strokeWidth="2" strokeLinecap="round"/>
    </svg>
  );

  return (
    <>

      <div className="lg:hidden fixed top-0 left-0 right-0 z-50 bg-[#11161D] flex items-center justify-between px-4"
           style={{ height: '56px', boxSizing: 'border-box' }}>
        <span className="text-xl font-bold text-[#FC563C]">Admin Panel</span>
        <button
          onClick={toggleMobile}
          style={{
            background: 'none',
            border: 'none',
            padding: '6px',
            cursor: 'pointer',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            flexShrink: 0,
          }}
          aria-label="Toggle menu"
        >
          {mobileOpen ? <CloseIcon /> : <HamburgerIcon />}
        </button>
      </div>

      <div
        className="lg:hidden fixed left-0 right-0 z-40 bg-[#11161D] overflow-hidden transition-all duration-300"
        style={{
          top: '56px',
          maxHeight: mobileOpen ? '400px' : '0px',
          boxSizing: 'border-box',
        }}
      >
        <nav className="flex flex-col gap-1 px-4 py-3">
          {menuItems.map((item, index) => (
            <Link
              key={index}
              to={item.to}
              onClick={() => setMobileOpen(false)}
              className="flex items-center gap-3 px-3 py-3 rounded-lg text-gray-400 hover:text-[#FC563C] hover:bg-white/5 transition-colors duration-200"
            >
              {item.icon}
              <span className="text-sm font-medium">{item.name}</span>
            </Link>
          ))}
        </nav>
      </div>

      {mobileOpen && (
        <div
          className="lg:hidden fixed inset-0 z-30 bg-black/40"
          style={{ top: '56px' }}
          onClick={() => setMobileOpen(false)}
        />
      )}

      <div className="lg:hidden" style={{ height: '56px' }} />


      <div
        className={`hidden lg:flex bg-[#11161D] p-6 flex-col min-h-screen transition-all duration-300 ${
          isOpen ? 'w-64' : 'w-17'
        }`}
      >
        <h1
          className={`text-2xl fixed duration-300 transition-all font-bold text-[#FC563C] mb-10 ${
            isOpen ? 'scale-x-100' : 'text-[0px] scale-x-0'
          }`}
        >
          Admin Panel
        </h1>

        <p
          onClick={toggleMenu}
          className={`cursor-pointer fixed text-[20px] text-gray-400 font-bold mb-10 transition-all duration-300 ${
            isOpen ? 'self-end' : 'self-center'
          }`}
        >
          {isOpen ? '◁' : '▷'}
        </p>

        <nav className="flex flex-col gap-6 fixed mt-20">
          {menuItems.map((item, index) => (
            <Link
              key={index}
              to={item.to}
              className="relative flex items-center gap-3 group hover:text-[#FC563C] cursor-pointer"
            >
              {item.icon}

              <span
                className={`transition-all duration-300 overflow-hidden ${
                  isOpen ? 'opacity-100 scale-x-100' : 'opacity-0 scale-x-0'
                }`}
              >
                {item.name}
              </span>

              {!isOpen && (
                <span className="absolute left-10 ml-2 whitespace-nowrap bg-gray-800 text-white text-sm px-2 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity duration-200">
                  {item.name}
                </span>
              )}
            </Link>
          ))}
        </nav>
      </div>
    </>
  );
};

export default AdminSideMenu;