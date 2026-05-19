const Loader = ({ size = 40 }) => {
  return (
    <div className="flex items-center justify-center">
      <div
        style={{ width: size, height: size }}
        className="border-4 border-gray-600 border-t-[#FC563C] rounded-full animate-spin"
      />
    </div>
  );
};

export default Loader;