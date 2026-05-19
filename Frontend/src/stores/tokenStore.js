import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export const useTokens = create(persist((set) => ({
    accessToken: "",
    refreshToken: "",
    setAccessToken: (token) => set((state) => ({ ...state, accessToken: token})),
    setRefreshToken: (token) => set((state) => ({ ...state, refreshToken: token})),
    clearTokens: () => set((state) => ({ ...state, accessToken: "", refreshToken: ""}))
    }), {name: "tokens"}
))