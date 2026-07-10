import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Register from './pages/Register';
import Home from './pages/Home';
import Dashboard from './pages/Dashboard';
import TakeExam from './pages/TakeExam';
import Feedback from './pages/Feedback';

function App() {
  return (
    <Router>
      <Routes>
        {/* Khi gõ localhost:5173/ thì vào Home */}
        <Route path="/" element={<Home />} />

        {/* Đăng ký / Đăng nhập */}
        <Route path="/register" element={<Register />} />

        {/* Bảng điều khiển học viên */}
        <Route path="/dashboard" element={<Dashboard />} />

        {/* Làm bài thi */}
        <Route path="/exam/:id" element={<TakeExam />} />

        {/* Phản hồi & Kết quả */}
        <Route path="/feedback/:attemptId" element={<Feedback />} />
      </Routes>
    </Router>
  );
}

export default App;