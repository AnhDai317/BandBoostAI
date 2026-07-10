import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const TakeExam = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    
    const [exam, setExam] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');
    
    // Save answers: key is questionId, value is answer text
    const [answers, setAnswers] = useState({});
    const [timeLeft, setTimeLeft] = useState(0);
    const [isSubmitting, setIsSubmitting] = useState(false);
    
    const timerRef = useRef(null);

    useEffect(() => {
        const token = localStorage.getItem("token");
        if (!token) {
            navigate("/register");
            return;
        }

        const fetchExam = async () => {
            try {
                const res = await fetch(`http://localhost:5229/api/Exams/${id}`, {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                if (!res.ok) throw new Exception("Không thể lấy thông tin đề thi.");
                
                const data = await res.json();
                setExam(data);
                
                // Set initial timer
                setTimeLeft(data.durationInMinutes * 60);
                
                // Initialize answers
                const initialAnswers = {};
                data.sections.forEach(s => {
                    s.questions.forEach(q => {
                        initialAnswers[q.id] = "";
                    });
                });
                setAnswers(initialAnswers);
            } catch (err) {
                console.error("Lỗi lấy đề thi:", err);
                setError("Lỗi đồng bộ đề thi. Vui lòng thử lại.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchExam();
    }, [id, navigate]);

    // Timer countdown logic
    useEffect(() => {
        if (timeLeft <= 0 && exam) {
            // Auto submit when time runs out
            handleAutoSubmit();
            return;
        }

        if (exam) {
            timerRef.current = setInterval(() => {
                setTimeLeft(prev => prev - 1);
            }, 1000);
        }

        return () => {
            if (timerRef.current) clearInterval(timerRef.current);
        };
    }, [timeLeft, exam]);

    const handleAnswerChange = (questionId, value) => {
        setAnswers(prev => ({
            ...prev,
            [questionId]: value
        }));
    };

    const formatTime = (seconds) => {
        const h = Math.floor(seconds / 3600);
        const m = Math.floor((seconds % 3600) / 60);
        const s = seconds % 60;
        return `${h > 0 ? h + ':' : ''}${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
    };

    const buildPayloadAnswers = () => {
        return Object.keys(answers).map(qId => ({
            questionId: qId,
            answerText: answers[qId]
        }));
    };

    const handleAutoSubmit = () => {
        alert("Hết giờ làm bài! Hệ thống đang tự động nộp bài của bạn.");
        submitExamAnswers(true);
    };

    const handleSubmitClick = (e) => {
        e.preventDefault();
        if (window.confirm("Bạn có chắc chắn muốn nộp bài thi này không?")) {
            submitExamAnswers(false);
        }
    };

    const submitExamAnswers = async (isAuto = false) => {
        if (isSubmitting) return;
        setIsSubmitting(true);
        if (timerRef.current) clearInterval(timerRef.current);

        const token = localStorage.getItem("token");
        const payload = {
            answers: buildPayloadAnswers()
        };

        try {
            const res = await fetch(`http://localhost:5229/api/Exams/${id}/submit`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (res.ok) {
                const attemptData = await res.json();
                navigate(`/feedback/${attemptData.id}`);
            } else {
                const errData = await res.json();
                alert("Lỗi nộp bài: " + (errData.message || "Không xác định"));
                setIsSubmitting(false);
            }
        } catch (err) {
            console.error("Lỗi nộp bài:", err);
            alert("Lỗi kết nối máy chủ khi nộp bài.");
            setIsSubmitting(false);
        }
    };

    if (isLoading) {
        return (
            <div className="min-h-screen bg-slate-900 flex justify-center items-center text-white">
                <svg className="animate-spin h-10 w-10 text-violet-500 mb-4" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                </svg>
                <span className="ml-2 font-medium text-slate-400">Đang khởi tạo phòng thi...</span>
            </div>
        );
    }

    if (error || !exam) {
        return (
            <div className="min-h-screen bg-slate-950 flex flex-col justify-center items-center text-white p-6">
                <span className="material-symbols-outlined text-rose-500 text-5xl mb-4">report</span>
                <p className="text-rose-400 font-bold mb-4">{error || "Không tìm thấy đề thi."}</p>
                <button onClick={() => navigate("/dashboard")} className="px-5 py-2.5 bg-violet-600 rounded-xl font-bold text-sm">
                    Về Bảng Điều Khiển
                </button>
            </div>
        );
    }

    const firstSection = exam.sections[0];
    const isWriting = exam.category === 3;

    // Helper for word count
    const getWordCount = (text) => {
        if (!text) return 0;
        return text.trim().split(/\s+/).filter(w => w.length > 0).length;
    };

    return (
        <div className="h-screen flex flex-col bg-slate-950 text-slate-100 font-sans overflow-hidden">
            {/* Exam Header */}
            <header className="h-16 border-b border-slate-800 bg-slate-900 px-6 flex justify-between items-center z-10">
                <div>
                    <h2 className="font-extrabold text-sm text-slate-200 line-clamp-1">{exam.title}</h2>
                    <p className="text-xs text-slate-400 font-light">Thí sinh: {JSON.parse(localStorage.getItem("user"))?.fullName}</p>
                </div>
                <div className="flex items-center gap-6">
                    <div className="flex items-center gap-2 bg-slate-800/80 border border-slate-700 px-4 py-1.5 rounded-full shadow-inner">
                        <span className="material-symbols-outlined text-sm text-amber-400 animate-pulse">timer</span>
                        <span className="text-sm font-mono font-bold tracking-wider text-amber-300">{formatTime(timeLeft)}</span>
                    </div>
                    <button
                        onClick={handleSubmitClick}
                        disabled={isSubmitting}
                        className="px-5 py-2 bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700 disabled:opacity-50 text-white font-bold text-xs rounded-xl shadow-md transition-all active:scale-95 flex items-center gap-1.5"
                    >
                        {isSubmitting ? "Đang nộp..." : "Nộp bài thi"}
                        <span className="material-symbols-outlined text-xs">done_all</span>
                    </button>
                </div>
            </header>

            {/* Split Screen Panel */}
            <main className="flex-1 flex overflow-hidden">
                {/* Left Panel: Shared Content (Passage / Prompt) */}
                <div className="w-1/2 border-r border-slate-800 p-8 overflow-y-auto bg-slate-900/30 flex flex-col space-y-6">
                    <div className="prose prose-invert max-w-none text-slate-300 font-light leading-relaxed">
                        <h3 className="text-slate-100 font-extrabold text-lg mb-4 uppercase tracking-wide border-b border-slate-800 pb-2">
                            {firstSection?.title || "Nội dung đề bài"}
                        </h3>
                        <div
                            className="space-y-4"
                            dangerouslySetInnerHTML={{ __html: firstSection?.sharedContent || "Đang cập nhật..." }}
                        />
                    </div>
                </div>

                {/* Right Panel: Inputs / Answer Sheet */}
                <div className="w-1/2 p-8 overflow-y-auto bg-slate-950 flex flex-col space-y-8">
                    <h3 className="text-slate-100 font-extrabold text-lg uppercase tracking-wide border-b border-slate-800 pb-2 flex items-center gap-2">
                        <span className="material-symbols-outlined text-violet-400">edit_square</span>
                        Bản trả lời của thí sinh
                    </h3>

                    {isWriting ? (
                        // Writing Task: Big Text Area with Word Count
                        firstSection?.questions.map((question) => {
                            const wordCount = getWordCount(answers[question.id]);
                            return (
                                <div key={question.id} className="space-y-4 flex-1 flex flex-col min-h-[400px]">
                                    <div className="flex justify-between items-center text-xs text-slate-400">
                                        <span>Hãy nhập bài luận của bạn bên dưới (tối thiểu 250 từ)</span>
                                        <span className={`font-bold ${wordCount >= 250 ? 'text-emerald-400' : 'text-amber-400'}`}>
                                            {wordCount} từ
                                        </span>
                                    </div>
                                    <textarea
                                        value={answers[question.id] || ""}
                                        onChange={(e) => handleAnswerChange(question.id, e.target.value)}
                                        placeholder="Nhập nội dung bài viết của bạn tại đây..."
                                        className="flex-1 w-full bg-slate-900 border border-slate-800 focus:border-violet-600 rounded-2xl p-6 outline-none text-slate-200 leading-relaxed font-mono resize-none focus:ring-4 focus:ring-violet-500/10 transition-all text-sm"
                                    />
                                </div>
                            );
                        })
                    ) : (
                        // Reading/Listening: Multiple choices or input fields
                        <div className="space-y-8">
                            {exam.sections.map((section, sIdx) => (
                                <div key={section.id} className="space-y-6">
                                    {section.questions.map((question) => {
                                        let qData = {};
                                        try {
                                            qData = JSON.parse(question.ContentJson);
                                        } catch (e) {
                                            qData = { text: "Câu hỏi bị lỗi cấu trúc dữ liệu." };
                                        }

                                        return (
                                            <div key={question.id} className="bg-slate-900/40 border border-slate-800 p-6 rounded-2xl space-y-4">
                                                <div className="flex items-start gap-3">
                                                    <span className="bg-violet-600 text-white font-black text-xs px-2.5 py-1 rounded-lg">
                                                        Câu {question.QuestionNumber}
                                                    </span>
                                                    <p className="text-slate-200 text-sm font-semibold leading-relaxed">
                                                        {qData.text}
                                                    </p>
                                                </div>

                                                {qData.options ? (
                                                    // Multiple choice rendering
                                                    <div className="grid grid-cols-2 gap-3 pl-10">
                                                        {qData.options.map((opt, oIdx) => {
                                                            const isSelected = answers[question.id] === opt;
                                                            return (
                                                                <button
                                                                    key={oIdx}
                                                                    type="button"
                                                                    onClick={() => handleAnswerChange(question.id, opt)}
                                                                    className={`p-3 rounded-xl border text-xs font-bold text-left transition-all flex items-center gap-2 ${isSelected ? 'bg-violet-600 border-violet-500 text-white shadow-md' : 'bg-slate-900/80 border-slate-800 text-slate-300 hover:bg-slate-900'}`}
                                                                >
                                                                    <span className={`w-5 h-5 rounded-full flex items-center justify-center border text-[10px] font-black ${isSelected ? 'bg-white text-violet-600 border-white' : 'border-slate-700 bg-slate-800 text-slate-400'}`}>
                                                                        {String.fromCharCode(65 + oIdx)}
                                                                    </span>
                                                                    {opt}
                                                                </button>
                                                            );
                                                        })}
                                                    </div>
                                                ) : (
                                                    // Short Answer / Fill in the blank
                                                    <div className="pl-10">
                                                        <input
                                                            type="text"
                                                            value={answers[question.id] || ""}
                                                            onChange={(e) => handleAnswerChange(question.id, e.target.value)}
                                                            placeholder="Nhập câu trả lời của bạn..."
                                                            className="w-full bg-slate-900 border border-slate-800 focus:border-violet-600 rounded-xl px-4 py-2.5 outline-none text-slate-200 focus:ring-4 focus:ring-violet-500/10 transition-all text-xs"
                                                        />
                                                    </div>
                                                )}
                                            </div>
                                        );
                                    })}
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
};

export default TakeExam;
