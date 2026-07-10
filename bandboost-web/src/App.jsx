import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Register from './pages/Register';
import Home from './pages/Home';
import Dashboard from './pages/Dashboard';
import TakeExam from './pages/TakeExam';
import Feedback from './pages/Feedback';
import Onboarding from './pages/Onboarding';
import Vocabulary from './pages/Vocabulary';
import Practice from './pages/Practice';

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

        {/* Cá nhân hóa mục tiêu và lộ trình học */}
        <Route path="/onboarding" element={<Onboarding />} />

        {/* Flashcard từ vựng theo chủ đề và trình độ */}
        <Route path="/vocabulary" element={<Vocabulary />} />

        {/* Trung tâm luyện bốn kỹ năng */}
        <Route path="/practice" element={<Practice />} />
      </Routes>
    </Router>
  );
}

export default App;
