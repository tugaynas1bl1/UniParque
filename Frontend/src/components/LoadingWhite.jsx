import React, { useEffect, useState } from "react";

const LoadingWhite = () => {
    return (
        <div className="w-full h-screen flex items-center justify-center">
            <div className="animate-spin rounded-full h-10 w-10 border-t-2 border-white"></div>
        </div>
    )
}

export default LoadingWhite;