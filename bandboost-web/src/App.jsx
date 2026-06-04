import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Register from './pages/Register';
import Home from './pages/Home'; // Import thêm trang Home

function App() {
  return (
    <Router>
      <Routes>
        {/* Khi gõ localhost:5173/ thì vào Home */}
        <Route path="/" element={<Home />} />

        {/* Khi gõ localhost:5173/register hoặc bấm nút từ Home thì sang đây */}
        <Route path="/register" element={<Register />} />
      </Routes>
    </Router>
  );
}

export default App;