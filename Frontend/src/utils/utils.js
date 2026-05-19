import axios from "axios"
import { useTokens } from "../stores/tokenStore"
import api from "./axios"

export const refreshTokens = async () => {
    try {
        const refreshToken = useTokens.getState().refreshToken
        const setAccessToken = useTokens.getState().setAccessToken
        const {data, statusText} = await api.post("auth/refresh", { refreshToken })
        if (statusText === "OK"){
            setAccessToken(data.data.accessToken)
            return data.data.accessToken
        }
    } catch (error) {
        console.error(error)
    }
}
 