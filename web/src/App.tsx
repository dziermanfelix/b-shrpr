import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import AddUser from './pages/AddUser'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/add" element={<AddUser />} />
      </Routes>
    </BrowserRouter>
  )
}
