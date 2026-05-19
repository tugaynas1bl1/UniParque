import React, { useEffect, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useTokens } from '../stores/tokenStore'
import api from '../utils/axios'
import { Link, useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'
import Loader from '../components/Loader'
import ChangePassword from '../components/ChangePassword'
import DeleteModal from '../components/DeleteModal'
import defUserImg from '../assets/user.png'


const Profile = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { accessToken, clearTokens } = useTokens()
  const fileInputRef = useRef(null)

  const [profilePhoto, setProfilePhoto] = useState("")
  const [userData, setUserData] = useState({ firstName: "", lastName: "" })
  const [savedUserData, setSavedUserData] = useState({ firstName: "", lastName: "" })
  const [selectedFile, setSelectedFile] = useState(null)
  const [isPhotoChanged, setIsPhotoChanged] = useState(false)
  const [isBtnDisabled, setIsBtnDisabled] = useState(true)
  const [loading, setLoading] = useState(false)
  const [changePasswordVisible, setChangePasswordVisible] = useState(false)
  const [deleteModalVisible, setDeleteModalVisible] = useState(false)

  useEffect(() => {
    if (!accessToken) navigate('/')
    getUserPhoto()
    getUserInfo()
  }, [])

  useEffect(() => {
    const hasChanges =
      savedUserData.firstName.trim() !== userData.firstName.trim() ||
      savedUserData.lastName.trim() !== userData.lastName.trim() ||
      isPhotoChanged
    setIsBtnDisabled(!hasChanges)
  }, [userData, savedUserData, isPhotoChanged])

  const getUserPhoto = async () => {
    try {
      const { data } = await api.get('auth/profile-photo')
      setProfilePhoto(data.data.url)
    } catch (err) { console.error(err) }
  }

  const getUserInfo = async () => {
    try {
      const { data, statusText } = await api.get('auth/my-info')
      if (statusText === "OK") {
        setUserData({ firstName: data.data.firstName, lastName: data.data.lastName })
        setSavedUserData({ firstName: data.data.firstName, lastName: data.data.lastName })
      }
    } catch (err) { console.error(err) }
  }

  const handleInput = (key, value) => setUserData(prev => ({ ...prev, [key]: value }))

  const handleFileChange = (e) => {
    const file = e.target.files[0]
    if (file) {
      setProfilePhoto(URL.createObjectURL(file))
      setSelectedFile(file)
      setIsPhotoChanged(true)
    }
  }

  const setProfilePicture = async (image) => {
    try {
      setLoading(true)
      const formData = new FormData()
      formData.append('file', image)
      await api.post('auth/add-photo', formData, {
        headers: { Authorization: `Bearer ${accessToken}`, 'Content-Type': 'multipart/form-data' }
      })
      toast.success(t('profile.photoSuccess'))
      getUserPhoto()
      setIsPhotoChanged(false)
      setSelectedFile(null)
    } catch (err) {
      console.error(err)
      toast.error(t('profile.photoFail'))
    } finally { setLoading(false) }
  }

  const updateUserInfo = async () => {
    try {
      setLoading(true)
      const { data } = await api.patch('auth/edit-profile', userData)
      if (data.success) {
        toast.success(t('profile.infoSuccess'))
        getUserInfo()
      }
    } catch (err) {
      console.error(err)
      toast.error(t('profile.infoFail'))
    } finally { setLoading(false) }
  }

  const saveChangesClick = async () => {
    if (isPhotoChanged && selectedFile) await setProfilePicture(selectedFile)

    if (savedUserData.firstName.trim() !== userData.firstName.trim() ||
        savedUserData.lastName.trim() !== userData.lastName.trim()) {
      if (userData.firstName.trim().length < 2) return toast.error(t('profile.firstnameMin'))
      if (userData.lastName.trim().length < 2) return toast.error(t('profile.lastnameMin'))
      await updateUserInfo()
    }
  }

  const handlePhotoDelete = async () => {
    try {
      const { data, statusText } = await api.delete('auth/delete-photo')
      if (statusText === "OK") {
        setProfilePhoto("")
        toast.success(t('profile.deletePhotoSuccess'))
      }
    } catch (err) { toast.error(t('profile.photoFail')); console.error(err) }
  }

  const handleDeleteAccount = async () => {
    setDeleteModalVisible(true)
  }

  const confirmDeleteAccount = async () => {
    setLoading(true)
    try {
      const { data } = await api.delete('auth/delete-account', {
        headers: { Authorization: `Bearer ${accessToken}` }
      })
      if (data.success) {
        toast.success(t('profile.deleteAccountSuccess'))
        clearTokens()
        navigate('/login')
      } else {
        toast.error(t('profile.deleteAccountFail'))
      }
    } catch (err) {
      console.error(err)
      toast.error(t('profile.deleteAccountFail'))
    } finally {
      setLoading(false)
      setDeleteModalVisible(false)
    }
  }

  const handleLogout = () => {
    clearTokens()
    navigate("/")
  }

  return (
    <div className="min-h-screen bg-[#0B0F14] flex flex-col justify-center items-center px-4 sm:px-6 md:px-10 select-none">

      <div className='flex flex-col md:flex-row w-full justify-between items-center md:items-start px-4 md:px-20 mb-6'>
        <Link to='/' className='text-white cursor-pointer hover:scale-110 transition-all duration-300 hover:text-orange-600 mb-4 md:mb-0'>
          {t('profile.home')}
        </Link>
        <button onClick={handleLogout} className="cursor-pointer text-white w-full md:w-40 h-12 bg-[#FC563C] rounded-lg hover:bg-white hover:text-[#FC563C] transition-all duration-300">
          {t('profile.logout')}
        </button>
      </div>

      {changePasswordVisible && <ChangePassword />}

      <div className="w-full max-w-2xl bg-white/5 backdrop-blur-xl border border-white/10 rounded-2xl p-6 sm:p-8 shadow-2xl select-none">
        <h1 className="text-3xl font-bold text-white mb-6 sm:mb-8 text-center">{t('profile.editProfile')}</h1>

        <div className="flex flex-col items-center gap-4 mb-6 sm:mb-8">
          {profilePhoto === "" ? (
            <div className='w-24 h-24 sm:w-28 sm:h-28 border-4 border-[#FC563C] rounded-full flex justify-center items-center relative'>
              <img src={defUserImg} alt="" />
            </div>
          ) : (
            <img src={profilePhoto} className="w-24 h-24 sm:w-28 sm:h-28 rounded-full object-cover border-4 border-[#FC563C]" alt="profile"/>
          )}
          
          <div className="flex flex-col sm:flex-row gap-4 sm:gap-10 text-sm">
            <button onClick={() => fileInputRef.current.click()} className="text-gray-400 hover:text-[#6c95c8] hover:scale-110 cursor-pointer transition-all duration-500">
              {t('profile.updatePhoto')}
            </button>
            <input type="file" ref={fileInputRef} onChange={handleFileChange} style={{ display: 'none' }} accept="image/*" />
            {profilePhoto && <button onClick={handlePhotoDelete} className="text-gray-400 hover:text-red-600 hover:scale-110 cursor-pointer transition-all duration-500">
              {t('profile.deletePhoto')}
            </button>}
          </div>
        </div>

        <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
          <input placeholder={t('profile.firstName')} value={userData.firstName} onChange={(e) => handleInput("firstName", e.target.value)} className="bg-transparent border border-white/20 rounded-lg px-4 py-3 text-white outline-none focus:border-[#FC563C]" />
          <input placeholder={t('profile.lastName')} value={userData.lastName} onChange={(e) => handleInput("lastName", e.target.value)} className="bg-transparent border border-white/20 rounded-lg px-4 py-3 text-white outline-none focus:border-[#FC563C]" />
        </div>

        <div className="flex flex-col gap-4 mt-6 sm:mt-8">
          <button disabled={isBtnDisabled} onClick={saveChangesClick} className={`${isBtnDisabled ? 'bg-gray-400 cursor-not-allowed opacity-50' : 'bg-gradient-to-r from-[#FC563C] to-[#ff8a70] hover:scale-105'} py-3 flex justify-center rounded-lg text-white font-semibold transition-all duration-500 relative`}>
            {t('profile.saveChanges')}
            {loading && <Loader size={20} className="absolute right-20 mt-1"/>}
          </button>

          <div onClick={() => setChangePasswordVisible(true)} className="text-gray-400 flex justify-center gap-3 cursor-pointer hover:bg-gray-950/20 w-full sm:w-52 p-3 rounded-md mx-auto transition-all duration-200">
            🔑 {t('profile.changePassword')}
          </div>

          <button onClick={handleDeleteAccount} className="text-white bg-red-600 rounded-lg py-3 mt-2 sm:mt-4 hover:bg-red-800 transition-all duration-300">
            {t('profile.deleteAccount')}
          </button>
        </div>
      </div>

      {deleteModalVisible && (
        <DeleteModal
          title={t('profile.deleteAccount')}
          message={t('profile.deleteAccountConfirm')}
          loading={loading}
          onCancel={() => setDeleteModalVisible(false)}
          onConfirm={confirmDeleteAccount}
        />
      )}

    </div>
  )
}

export default Profile