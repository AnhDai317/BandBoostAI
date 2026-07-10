import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';

const Dashboard = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState(null);
    const [exams, setExams] = useState([]);
    const [attempts, setAttempts] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const token = localStorage.getItem("token");
        const userData = localStorage.getItem("user");
        if (!token || !userData) {
            navigate("/register");
            return;
        }

        setUser(JSON.parse(userData));

        // Fetch data
        const fetchData = async () => {
            try {
                // Fetch Exams
                const examsRes = await fetch("http://localhost:5229/api/Exams", {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                const examsData = await examsRes.json();
                setExams(examsData);

                // Fetch User Attempts
                const attemptsRes = await fetch("http://localhost:5229/api/Exams/my-attempts", {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                if (attemptsRes.ok) {
                    const attemptsData = await attemptsRes.json();
                    setAttempts(attemptsData);
                }
            } catch (err) {
                console.error("Lỗi tải dữ liệu:", err);
                setError("Không thể đồng bộ dữ liệu từ hệ thống.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
    }, [navigate]);

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        navigate("/");
    };

    if (isLoading) {
        return (
            <div className="min-h-screen bg-slate-900 flex flex-col justify-center items-center text-white">
                <svg className="animate-spin h-10 w-10 text-violet-500 mb-4" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                </svg>
                <p className="text-slate-400 font-medium">Đang tải thông tin bảng điều khiển...</p>
            </div>
        );
    }

    // Get category badge style
    const getCategoryStyle = (category) => {
        switch (category) {
            case 1: return "bg-sky-500/10 text-sky-400 border-sky-500/20"; // Reading
            case 2: return "bg-emerald-500/10 text-emerald-400 border-emerald-500/20"; // Listening
            case 3: return "bg-amber-500/10 text-amber-400 border-amber-500/20"; // Writing
            case 4: return "bg-rose-500/10 text-rose-400 border-rose-500/20"; // Speaking
            default: return "bg-slate-500/10 text-slate-400 border-slate-500/20";
        }
    };

    const getCategoryName = (category) => {
        switch (category) {
            case 1: return "Reading";
            case 2: return "Listening";
            case 3: return "Writing";
            case 4: return "Speaking";
            default: return "General";
        }
    };

    // Calculate current band (highest or latest)
    const latestAttempt = attempts[0];
    const currentBand = latestAttempt ? parseFloat(latestAttempt.score).toFixed(1) : "0.0";
    const targetBand = "7.5"; // Hardcoded mockup for target

    return (
        <div className="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans">
            {/* Header */}
            <header className="border-b border-slate-800/80 bg-slate-900/60 backdrop-blur-md sticky top-0 z-40">
                <div className="max-w-7xl mx-auto px-6 h-20 flex justify-between items-center">
                    <div className="flex items-center gap-3">
                        <span className="text-2xl font-black bg-gradient-to-r from-violet-400 to-indigo-400 bg-clip-text text-transparent">
                            BandBoost AI
                        </span>
                    </div>
                    <div className="flex items-center gap-6">
                        <div className="hidden sm:flex flex-col text-right">
                            <span className="font-bold text-sm text-slate-200">{user?.fullName}</span>
                            <span className="text-xs text-slate-400">{user?.email}</span>
                        </div>
                        <button
                            onClick={handleLogout}
                            className="px-4 py-2 text-xs font-bold uppercase tracking-wider text-slate-400 border border-slate-800 rounded-xl hover:bg-slate-800 hover:text-white transition-all flex items-center gap-2"
                        >
                            <span className="material-symbols-outlined text-sm">logout</span> Đăng xuất
                        </button>
                    </div>
                </div>
            </header>

            {/* Dashboard Content */}
            <main className="flex-1 max-w-7xl mx-auto w-full p-6 space-y-10">
                {/* Error Banner */}
                {error && (
                    <div className="p-4 bg-rose-500/10 border border-rose-500/20 text-rose-400 rounded-2xl flex items-center gap-3">
                        <span className="material-symbols-outlined">report</span>
                        <p className="text-sm font-semibold">{error}</p>
                    </div>
                )}

                {/* Banner / Welcome */}
                <section className="bg-gradient-to-r from-violet-900/40 via-indigo-900/40 to-slate-900 border border-violet-500/20 rounded-[2.5rem] p-8 md:p-12 relative overflow-hidden flex flex-col md:flex-row md:items-center justify-between gap-8 shadow-xl">
                    <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-violet-500/10 via-transparent to-transparent"></div>
                    <div className="z-10 space-y-4">
                        <div className="inline-flex items-center gap-2 px-3.5 py-1 rounded-full bg-violet-500/10 border border-violet-500/20">
                            <span className="w-2 h-2 rounded-full bg-violet-400 animate-ping"></span>
                            <span className="text-xs font-bold uppercase tracking-widest text-violet-300">IELTS Preparation Hub</span>
                        </div>
                        <h2 className="text-3xl md:text-4xl font-black tracking-tight text-white leading-tight">
                            Chào mừng trở lại,<br/>
                            <span className="bg-gradient-to-r from-violet-300 to-indigo-300 bg-clip-text text-transparent">{user?.fullName}</span>!
                        </h2>
                        <p className="text-slate-400 font-light max-w-lg">
                            Hệ thống AI đã cập nhật điểm thi thử của bạn. Hãy chọn một bài luyện tập mới bên dưới để tiếp tục nâng band điểm.
                        </p>
                    </div>

                    {/* Stats */}
                    <div className="z-10 flex gap-6 sm:gap-10">
                        <div className="bg-slate-900/80 backdrop-blur border border-slate-800 p-6 rounded-3xl w-32 sm:w-40 text-center shadow-lg">
                            <p className="text-xs font-semibold uppercase tracking-widest text-slate-400">Hiện Tại</p>
                            <p className="text-4xl sm:text-5xl font-black mt-2 bg-gradient-to-br from-violet-400 to-indigo-400 bg-clip-text text-transparent">{currentBand}</p>
                            <p className="text-[10px] text-slate-500 mt-1">IELTS Band</p>
                        </div>
                        <div className="bg-slate-900/80 backdrop-blur border border-slate-800 p-6 rounded-3xl w-32 sm:w-40 text-center shadow-lg">
                            <p className="text-xs font-semibold uppercase tracking-widest text-slate-400">Mục Tiêu</p>
                            <p className="text-4xl sm:text-5xl font-black mt-2 text-indigo-400">{targetBand}</p>
                            <p className="text-[10px] text-slate-500 mt-1">IELTS Band</p>
                        </div>
                    </div>
                </section>

                <div className="grid lg:grid-cols-3 gap-10">
                    {/* Left 2 cols: Exams List */}
                    <div className="lg:col-span-2 space-y-6">
                        <div className="flex justify-between items-center">
                            <h3 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
                                <span className="material-symbols-outlined text-violet-400">assignment</span>
                                Danh sách Đề Thi Thử ({exams.length})
                            </h3>
                        </div>

                        {exams.length === 0 ? (
                            <div className="bg-slate-900/40 border border-slate-800/80 rounded-3xl p-12 text-center text-slate-500">
                                <span className="material-symbols-outlined text-4xl mb-2">inbox</span>
                                <p>Chưa có đề thi nào được tạo trên hệ thống.</p>
                            </div>
                        ) : (
                            <div className="grid sm:grid-cols-2 gap-6">
                                {exams.map((exam) => (
                                    <div
                                        key={exam.id}
                                        className="bg-slate-900/40 hover:bg-slate-900 border border-slate-800 hover:border-slate-700/80 rounded-3xl p-6 transition-all duration-300 flex flex-col justify-between group shadow-sm hover:shadow-md"
                                    >
                                        <div className="space-y-4">
                                            <div className="flex justify-between items-center">
                                                <span className={`px-3 py-1 rounded-full text-xs font-bold border ${getCategoryStyle(exam.category)}`}>
                                                    {getCategoryName(exam.category)}
                                                </span>
                                                <span className="text-xs text-slate-400 flex items-center gap-1">
                                                    <span className="material-symbols-outlined text-sm">schedule</span>
                                                    {exam.durationInMinutes} phút
                                                </span>
                                            </div>
                                            <h4 className="font-bold text-lg text-slate-100 group-hover:text-violet-400 transition-colors leading-snug">
                                                {exam.title}
                                            </h4>
                                            <p className="text-sm text-slate-400 font-light line-clamp-2">
                                                {exam.description || "Bài thi IELTS mô phỏng độ chính xác cao."}
                                            </p>
                                        </div>
                                        <div className="pt-6 border-t border-slate-800/60 mt-6 flex justify-between items-center">
                                            <span className="text-xs font-bold text-slate-500 uppercase tracking-wider">Mức độ: Free</span>
                                            <Link
                                                to={`/exam/${exam.id}`}
                                                className="px-4 py-2 bg-violet-600 hover:bg-violet-700 text-white font-bold text-xs rounded-xl transition-all shadow-md active:scale-95 flex items-center gap-1"
                                            >
                                                Làm bài <span className="material-symbols-outlined text-xs">arrow_forward</span>
                                            </Link>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    {/* Right col: Attempts History */}
                    <div className="space-y-6">
                        <h3 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
                            <span className="material-symbols-outlined text-indigo-400">history</span>
                            Lịch Sử Làm Bài
                        </h3>

                        {attempts.length === 0 ? (
                            <div className="bg-slate-900/40 border border-slate-800/80 rounded-3xl p-8 text-center text-slate-500 text-sm">
                                Bạn chưa tham gia bài thi thử nào. Hãy làm một bài thi ở cột bên trái để bắt đầu!
                            </div>
                        ) : (
                            <div className="space-y-4">
                                {attempts.map((attempt) => (
                                    <div
                                        key={attempt.id}
                                        className="bg-slate-900/40 border border-slate-800 rounded-2xl p-5 space-y-3 hover:bg-slate-900/70 transition-all"
                                    >
                                        <div className="flex justify-between items-start">
                                            <h4 className="font-bold text-sm text-slate-200 line-clamp-1 leading-snug">
                                                {attempt.examTitle}
                                            </h4>
                                            <span className="bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 px-2 py-0.5 rounded text-xs font-black">
                                                {parseFloat(attempt.score).toFixed(1)}
                                            </span>
                                        </div>
                                        <div className="flex justify-between items-center text-xs text-slate-400">
                                            <span>
                                                {new Date(attempt.createdAt).toLocaleDateString("vi-VN", {
                                                    day: "2-digit",
                                                    month: "2-digit",
                                                    year: "numeric"
                                                })}
                                            </span>
                                            <Link
                                                to={`/feedback/${attempt.id}`}
                                                className="text-violet-400 hover:text-violet-300 font-bold hover:underline flex items-center gap-0.5"
                                            >
                                                Xem nhận xét <span className="material-symbols-outlined text-xs">arrow_right</span>
                                            </Link>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
};

export default Dashboard;
