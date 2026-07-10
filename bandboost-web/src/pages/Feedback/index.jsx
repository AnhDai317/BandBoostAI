import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const Feedback = () => {
    const { attemptId } = useParams();
    const navigate = useNavigate();
    
    const [attempt, setAttempt] = useState(null);
    const [feedbackData, setFeedbackData] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const token = localStorage.getItem("token");
        if (!token) {
            navigate("/register");
            return;
        }

        const fetchAttempt = async () => {
            try {
                const res = await fetch(`http://localhost:5229/api/Exams/attempts/${attemptId}`, {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                if (!res.ok) throw new Error("Không tìm thấy kết quả làm bài.");
                
                const data = await res.json();
                setAttempt(data);
                
                // Parse feedback JSON
                if (data.feedback) {
                    try {
                        const parsedFeedback = JSON.parse(data.feedback);
                        setFeedbackData(parsedFeedback);
                    } catch (pe) {
                        console.error("Lỗi parse JSON phản hồi:", pe);
                        setFeedbackData({
                            overallBand: data.score,
                            detailedFeedback: data.feedback
                        });
                    }
                }
            } catch (err) {
                console.error("Lỗi lấy thông tin attempt:", err);
                setError("Không thể tải kết quả bài thi từ máy chủ.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchAttempt();
    }, [attemptId, navigate]);

    if (isLoading) {
        return (
            <div className="min-h-screen bg-slate-900 flex justify-center items-center text-white">
                <svg className="animate-spin h-10 w-10 text-violet-500 mb-4" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                </svg>
                <span className="ml-2 font-medium text-slate-400">Đang đồng bộ phân tích của AI...</span>
            </div>
        );
    }

    if (error || !attempt) {
        return (
            <div className="min-h-screen bg-slate-950 flex flex-col justify-center items-center text-white p-6">
                <span className="material-symbols-outlined text-rose-500 text-5xl mb-4">report</span>
                <p className="text-rose-400 font-bold mb-4">{error || "Không tìm thấy kết quả làm bài."}</p>
                <button onClick={() => navigate("/dashboard")} className="px-5 py-2.5 bg-violet-600 rounded-xl font-bold text-sm">
                    Về Bảng Điều Khiển
                </button>
            </div>
        );
    }

    // Try parsing answers
    let studentAnswers = [];
    try {
        studentAnswers = JSON.parse(attempt.answersJson);
    } catch {
        studentAnswers = [];
    }

    // Check if Writing criteria exist
    const criteria = feedbackData?.criteriaScores;

    return (
        <div className="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans">
            {/* Header */}
            <header className="border-b border-slate-800 bg-slate-900 px-6 h-16 flex justify-between items-center">
                <div className="flex items-center gap-2">
                    <span className="text-lg font-bold text-slate-200">Báo cáo đánh giá - {attempt.examTitle}</span>
                </div>
                <button
                    onClick={() => navigate("/dashboard")}
                    className="px-4 py-2 text-xs font-bold bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl transition-all flex items-center gap-1"
                >
                    <span className="material-symbols-outlined text-sm">dashboard</span> Bảng điều khiển
                </button>
            </header>

            {/* Content Container */}
            <main className="flex-1 max-w-5xl mx-auto w-full p-6 space-y-8">
                {/* Score Summary Box */}
                <section className="bg-gradient-to-r from-violet-950/60 to-slate-900 border border-violet-500/20 rounded-[2rem] p-8 flex flex-col md:flex-row items-center gap-8 shadow-xl">
                    <div className="relative flex justify-center items-center">
                        <div className="absolute inset-0 bg-violet-500/10 rounded-full filter blur-xl animate-pulse"></div>
                        <div className="w-32 h-32 rounded-full border-4 border-violet-500/30 bg-slate-900 flex flex-col justify-center items-center relative z-10 shadow-lg">
                            <span className="text-[10px] uppercase tracking-widest text-slate-400 font-bold">IELTS Band</span>
                            <span className="text-5xl font-black bg-gradient-to-r from-violet-400 to-indigo-400 bg-clip-text text-transparent mt-1">
                                {parseFloat(attempt.score).toFixed(1)}
                            </span>
                        </div>
                    </div>

                    <div className="flex-1 space-y-3 text-center md:text-left">
                        <span className="text-xs font-bold uppercase tracking-widest text-violet-400">Đánh giá chung của AI</span>
                        <h2 className="text-2xl font-extrabold text-white tracking-tight">Kết quả đánh giá năng lực</h2>
                        <p className="text-slate-400 text-sm font-light leading-relaxed">
                            {feedbackData?.detailedFeedback || "Hệ thống đã nhận được đáp án và tính điểm thành công."}
                        </p>
                    </div>
                </section>

                {/* Sub-scores breakdown */}
                {criteria && (
                    <section className="bg-slate-900/40 border border-slate-850 p-6 rounded-3xl space-y-6">
                        <h3 className="text-sm font-bold uppercase tracking-wider text-slate-300 border-b border-slate-800 pb-3 flex items-center gap-2">
                            <span className="material-symbols-outlined text-violet-400">analytics</span>
                            Điểm số chi tiết theo 4 tiêu chí IELTS
                        </h3>
                        <div className="grid md:grid-cols-4 gap-6">
                            {[
                                { name: "Task Response", score: criteria.taskResponse },
                                { name: "Coherence & Cohesion", score: criteria.coherence },
                                { name: "Lexical Resource", score: criteria.lexicalResource },
                                { name: "Grammatical Range", score: criteria.grammar }
                            ].map((item, idx) => (
                                <div key={idx} className="bg-slate-950/60 border border-slate-850 p-5 rounded-2xl text-center space-y-2">
                                    <p className="text-xs text-slate-400 font-semibold">{item.name}</p>
                                    <p className="text-3xl font-black text-violet-400">{parseFloat(item.score).toFixed(1)}</p>
                                    <div className="w-full bg-slate-800 h-1.5 rounded-full overflow-hidden">
                                        <div 
                                            className="bg-gradient-to-r from-violet-500 to-indigo-500 h-full rounded-full" 
                                            style={{ width: `${(item.score / 9) * 100}%` }}
                                        />
                                    </div>
                                </div>
                            ))}
                        </div>
                    </section>
                )}

                {/* Improvements and Suggestions */}
                {feedbackData?.improvements && feedbackData.improvements.length > 0 && (
                    <section className="bg-slate-900/40 border border-slate-850 p-6 rounded-3xl space-y-4">
                        <h3 className="text-sm font-bold uppercase tracking-wider text-slate-300 border-b border-slate-800 pb-3 flex items-center gap-2">
                            <span className="material-symbols-outlined text-emerald-400">trending_up</span>
                            Gợi ý cải thiện điểm số
                        </h3>
                        <ul className="space-y-3.5 pl-2">
                            {feedbackData.improvements.map((item, idx) => (
                                <li key={idx} className="text-sm text-slate-300 font-light flex items-start gap-3">
                                    <span className="material-symbols-outlined text-emerald-400 text-lg">check_circle</span>
                                    <span>{item}</span>
                                </li>
                            ))}
                        </ul>
                    </section>
                )}

                {/* Submitted Answers / Essay */}
                <section className="bg-slate-900/40 border border-slate-850 p-6 rounded-3xl space-y-4">
                    <h3 className="text-sm font-bold uppercase tracking-wider text-slate-300 border-b border-slate-800 pb-3 flex items-center gap-2">
                        <span className="material-symbols-outlined text-indigo-400">subject</span>
                        Bài làm đã nộp
                    </h3>
                    <div className="bg-slate-950/80 rounded-2xl p-6 border border-slate-850 text-sm font-mono leading-relaxed text-slate-300 max-h-96 overflow-y-auto whitespace-pre-line">
                        {studentAnswers.length > 0 ? (
                            studentAnswers.map((ans, idx) => (
                                <div key={idx} className="space-y-2">
                                    {studentAnswers.length > 1 && (
                                        <p className="text-xs text-violet-400 font-bold border-b border-slate-800/80 pb-1 mt-2">
                                            Đáp án Câu Hỏi {idx + 1}:
                                        </p>
                                    )}
                                    <p className="text-slate-300 py-1">{ans.answerText || "(Trống)"}</p>
                                </div>
                            ))
                        ) : (
                            <p className="text-slate-500 italic">Không tìm thấy nội dung trả lời.</p>
                        )}
                    </div>
                </section>
            </main>
        </div>
    );
};

export default Feedback;
