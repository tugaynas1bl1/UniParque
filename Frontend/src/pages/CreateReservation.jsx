import React, { useEffect, useState } from 'react'
import api from '../utils/axios'
import ParkingBranch from '../components/ParkingBranch'
import { useTranslation } from 'react-i18next'

const CreateReservation = () => {
  const { t } = useTranslation()
  const [branches, setBranches] = useState([])
  const [search, setSearch] = useState('')
  const [filteredBranches, setFilteredBranches] = useState([])

  const getBranches = async () => {
    try {
      const { data, statusText } = await api.get('ParkingBranches/all')
      if (statusText === 'OK') {
        setBranches(data.data)
        setFilteredBranches(data.data)
      }
    } catch (error) {
      console.error(error)
    }
  }

  useEffect(() => {
    getBranches()
  }, [])

  useEffect(() => {
    const filtered = branches.filter(b =>
      (b.branchName || '').toLowerCase().includes(search.toLowerCase())
    )
    setFilteredBranches(filtered)
  }, [search, branches])

  return (
    <div className='min-h-screen bg-[#0B0F14] text-white px-6 py-32'>
      
      <div className='text-center mb-12'>
        <h1 className='text-4xl md:text-5xl font-bold'>
          {t('createReservation.title').split(' ')[0]}{' '}
          <span className='text-[#FC563C]'>
            {t('createReservation.highlight')}
          </span>
        </h1>
        <p className='text-gray-400 mt-4'>
          {t('createReservation.description')}
        </p>

        <input
          type='text'
          placeholder={t('createReservation.searchPlaceholder')}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className='mt-6 px-4 py-2 rounded-4xl border-[#FC563C] border text-white w-full max-w-md mx-auto block focus:outline-none focus:ring-2 focus:ring-[#FC563C]'
        />
      </div>

      <div className='grid sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-10 max-w-7xl mx-auto'>
        {filteredBranches.length > 0 ? (
          filteredBranches.map(b => (
            <ParkingBranch key={b.id} branch={b} />
          ))
        ) : (
          <p className='text-center text-gray-400 col-span-full mt-10'>
            {t('createReservation.noResults')}
          </p>
        )}
      </div>
    </div>
  )
}

export default CreateReservation