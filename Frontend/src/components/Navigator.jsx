import React from 'react'
import { Route, Routes } from 'react-router-dom'
import MainLayout from './MainLayout'
import Home from '../pages/Home'
import Login from '../pages/Login'
import Reservation from '../pages/Reservation'
import Profile from '../pages/Profile'
import CreateReservation from '../pages/CreateReservation'
import SelectPlace from '../pages/SelectPlace'
import ActiveReservations from '../pages/ActiveReservations'
import AdminDashboard from '../pages/AdminDashboard'
import UsersManagement from '../pages/UsersManagement'
import ReservationsManagement from '../pages/ReservationsManagement'
import BranchManagement from '../pages/BranchManagement'
import ChangePassword from './ChangePassword'
import Wallet from '../pages/Wallet'
import AdminFullLayout from '../pages/AdminFullLayout'
import Settings from '../pages/Settings'
import ForgotPassword from '../pages/ForgotPassword'
import PastReservations from '../pages/PastReservations'
import About from '../pages/About'
import AdminUserProfile from '../pages/AdminUserProfile'
import Contact from '../pages/Contact'

const Navigator = () => {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path='/' element={<Home />} />
        <Route path='/login' element={<Login />} />
        <Route path='/reservation' element={<Reservation />} />
        <Route path='/profile' element={<Profile />} />
        <Route path='/create-reservation' element={<CreateReservation />} />
        <Route path='/select-place/:branchId' element={<SelectPlace />} />
        <Route path='/my-active-reservations' element={<ActiveReservations />} />
        <Route path='/change-password' element={<ChangePassword />} />
        <Route path='/my-wallet' element={<Wallet />} />
        <Route path='/forgot-password' element={<ForgotPassword />} />
        <Route path='/past-reservations' element={<PastReservations />} />
        <Route path='/about' element={<About />} />
        <Route path='/contact' element={<Contact />} />


        
        <Route path='/admin'>
          <Route path='dashboard' element={<AdminDashboard />} />
          <Route path='manage-users' element={<UsersManagement />} />
          <Route path='manage-reservations' element={<ReservationsManagement />} />
          <Route path='manage-branches' element={<BranchManagement />} />
          <Route path='manage-places/:branchId' element={<AdminFullLayout />} />
          <Route path='settings' element={<Settings />} />
          <Route path="users/:id" element={<AdminUserProfile />} />
        </Route>

      </Route>
    </Routes>
  )
}

export default Navigator
