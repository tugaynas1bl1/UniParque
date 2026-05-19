import React, { useEffect } from 'react'
import Header from './components/Header'
import Navigator from './components/Navigator'

const App = () => {
  useEffect(() => {
    window.history.scrollRestoration = "manual"
  window.scrollTo(0, 0)
}, [])

  return (
    <div>
      <Navigator />
    </div>
  )
}

export default App
