import React, { useEffect } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import Header from './Header';
import { Toaster } from 'react-hot-toast';
import AdminSideMenu from '../components/AdminSideMenu';
import api from '../utils/axios';
import Footer from './Footer';

const MainLayout = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getActiveMenu = () => {

    if (location.pathname.startsWith('/select-place'))
      return "Reservation";

    switch(location.pathname){
      case '/': return 'Home';
      case '/login': return "Login";
      case '/reservation': return "Reservation";
      case '/register': return "Register";
      case '/profile': return "Profile";
      case '/create-reservation': return "Reservation";
      case '/my-active-reservations': return "Reservation";
      case '/past-reservations': return "Reservation";
      case '/dashboard': return "Dashboard";
      case '/forgot-password': return "ForgotPassword";
      case '/about': return "About";
      case '/contact' : return "Contact"
      default: return '';
    }
  };

  

  const checkAdmin = async () => {
    try {
      const { data } = await api.get('auth/am-i-admin');
      const isAdmin = data.data;

      if (location.pathname.startsWith('/admin') && !isAdmin) {
        navigate('/login', { replace: true });
      } else if (isAdmin && !location.pathname.startsWith('/admin')) {
        navigate('/admin/dashboard', { replace: true });
      }
    } catch (error) {
      console.error(error);
      navigate('/login', { replace: true });
    }
  };

  useEffect(() => {
    checkAdmin();
  }, [location.pathname, navigate]);

  if(location.pathname.startsWith('/admin')) {
    return (
      <div className="flex min-h-screen bg-gray-950 text-white">
        <AdminSideMenu />
        <div className="flex-1 p-8">
          <Outlet />
        </div>
        <Toaster toastOptions={{
          className: "w-200 font-bold text-[16px] px-6 py-4"
        }} position='top-right' />
      </div>
    );
  }

  return (
    <div>
      <Toaster toastOptions={{
        className: "w-200 font-bold text-[16px] px-6 py-4"
      }} position='top-right' />

      {!location.pathname.startsWith('/admin') && location.pathname !== '/login' &&
       location.pathname !== '/register' && location.pathname !== '/forgot-password' && location.pathname !== '/profile' &&
        <Header activeMenu={getActiveMenu()} />
      }

      <Outlet />
      {!location.pathname.startsWith('/admin') && location.pathname !== '/login' &&
       location.pathname !== '/register' && location.pathname !== '/forgot-password' && location.pathname !== '/profile' &&
        <Footer />
      }
    </div>
  );
};

export default MainLayout;