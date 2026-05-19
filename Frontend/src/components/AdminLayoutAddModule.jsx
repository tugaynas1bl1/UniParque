import React, { useState, useEffect } from 'react';

const AdminLayoutAddModule = ({ onDone, placeholder, setIsVisible, header }) => {
  const [inputValue, setInputValue] = useState('');

  useEffect(() => {
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = 'auto';
    };
  }, []);

  const handleCreate = () => {
    if (!inputValue) return

    onDone(inputValue);
    setInputValue('');
  };

  const handleCancel = () => {
    setInputValue('');
    setIsVisible(false)
  };

  return (
    // Overlay
    <div
      className="fixed inset-0 flex items-center justify-center bg-black/40 backdrop-blur-sm z-50"
      onClick={handleCancel}
    >
      <div
        className="w-[400px] bg-white rounded-2xl shadow-xl p-6 backdrop-blur-md animate-fadeIn"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-xl font-semibold mb-4 text-gray-800">{header}</h2>
        <input
          type="text"
          placeholder={placeholder}
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-orange-400 placeholder:text-gray-500 text-black mb-6"
        />
        <div className="flex justify-end gap-4">
          <button
            onClick={handleCancel}
            className="px-4 py-2 rounded-lg bg-gray-300 hover:bg-gray-400 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={handleCreate}
            className="px-4 py-2 rounded-lg bg-orange-500 text-white hover:bg-orange-600 transition-colors"
          >
            Create
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminLayoutAddModule;