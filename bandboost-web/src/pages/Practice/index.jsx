import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import LearningShell from "../../components/LearningShell";
import { apiRequest } from "../../services/api";

const skills = [
    { value: 1, name: "Reading", description: "Đọc hiểu, tìm ý chính và xử lý từ vựng trong ngữ cảnh.", icon: "menu_book", color: "sky" },
    { value: 2, name: "Listening", description: "Nghe chi tiết, nhận diện từ khóa và phản xạ với giọng nói tự nhiên.", icon: "headphones", color: "emerald" },
    { value: 3, name: "Writing", description: "Lập luận, phát triển ý và nhận chấm điểm chi tiết từ AI.", icon: "edit_note", color: "amber" },
    { value: 4, name: "Speaking", description: "Luyện diễn đạt trôi chảy, phát âm và mở rộng câu trả lời.", icon: "record_voice_over", color: "rose" }
];

const skillStyles = {
    sky: "border-sky-400/25 bg-sky-400/[.07] text-sky-300",
    emerald: "border-emerald-400/25 bg-emerald-400/[.07] text-emerald-300",
    amber: "border-amber-400/25 bg-amber-400/[.07] text-amber-300",
    rose: "border-rose-400/25 bg-rose-400/[.07] text-rose-300"
};

const Practice = () => {
    const navigate = useNavigate();
    const [exams, setExams] = useState([]);
    const [selectedSkill, setSelectedSkill] = useState(1);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!localStorage.getItem("token")) {
            navigate("/register");
            return;
        }
        apiRequest("/Exams")
            .then(setExams)
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [navigate]);

    const filteredExams = exams.filter(exam => exam.category === selectedSkill);
    const currentSkill = skills.find(skill => skill.value === selectedSkill);

    return (
        <LearningShell>
            <div className="mb-8">
                <p className="text-xs font-bold uppercase tracking-[.2em] text-violet-400">Practice hub</p>
                <h1 className="mt-2 text-3xl font-black tracking-tight md:text-4xl">Luyện kỹ năng trọng tâm</h1>
                <p className="mt-3 max-w-2xl text-slate-400">Chọn kỹ năng cần cải thiện, hoàn thành bài luyện và nhận phản hồi để lộ trình tự cập nhật.</p>
            </div>

            {error && <div className="mb-6 rounded-2xl border border-rose-400/20 bg-rose-400/10 p-4 text-sm text-rose-300">{error}</div>}

            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
                {skills.map(skill => (
                    <button key={skill.value} onClick={() => setSelectedSkill(skill.value)} className={`rounded-3xl border p-5 text-left transition hover:-translate-y-1 ${selectedSkill === skill.value ? skillStyles[skill.color] : "border-white/10 bg-white/[.035] text-slate-400 hover:border-white/20"}`}>
                        <span className="grid h-12 w-12 place-items-center rounded-2xl bg-white/10"><span className="material-symbols-outlined">{skill.icon}</span></span>
                        <h2 className="mt-4 font-black text-white">{skill.name}</h2>
                        <p className="mt-2 text-xs leading-5 text-slate-500">{skill.description}</p>
                        <span className="mt-4 block text-[11px] font-bold">{exams.filter(exam => exam.category === skill.value).length} bài luyện</span>
                    </button>
                ))}
            </div>

            <section className="mt-10">
                <div className="mb-5 flex items-center justify-between">
                    <div><p className="text-xs font-bold uppercase tracking-wider text-slate-500">Đang chọn</p><h2 className="mt-1 text-2xl font-black">{currentSkill.name}</h2></div>
                    <span className="rounded-full bg-white/5 px-4 py-2 text-xs font-semibold text-slate-400">{filteredExams.length} bài</span>
                </div>

                {loading ? (
                    <div className="grid min-h-60 place-items-center rounded-3xl border border-white/10 bg-white/[.025]"><span className="material-symbols-outlined animate-spin text-4xl text-violet-400">progress_activity</span></div>
                ) : filteredExams.length > 0 ? (
                    <div className="grid gap-5 md:grid-cols-2">
                        {filteredExams.map(exam => (
                            <article key={exam.id} className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                                <div className="flex items-center justify-between"><span className={`rounded-full border px-3 py-1 text-xs font-bold ${skillStyles[currentSkill.color]}`}>{currentSkill.name}</span><span className="flex items-center gap-1 text-xs text-slate-500"><span className="material-symbols-outlined text-base">schedule</span>{exam.durationInMinutes} phút</span></div>
                                <h3 className="mt-5 text-lg font-black">{exam.title}</h3>
                                <p className="mt-2 line-clamp-2 text-sm leading-6 text-slate-500">{exam.description}</p>
                                <Link to={`/exam/${exam.id}`} className="mt-6 flex items-center justify-center gap-2 rounded-2xl bg-violet-600 py-3.5 font-bold text-white transition hover:bg-violet-500">Bắt đầu luyện <span className="material-symbols-outlined">arrow_forward</span></Link>
                            </article>
                        ))}
                    </div>
                ) : (
                    <div className="rounded-3xl border border-dashed border-white/15 bg-white/[.025] p-10 text-center">
                        <span className="material-symbols-outlined text-5xl text-slate-700">construction</span>
                        <h3 className="mt-4 text-lg font-bold">Bài {currentSkill.name} đang được bổ sung</h3>
                        <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-slate-500">Hiện bạn có thể chọn Reading hoặc Writing để luyện ngay. Khu vực này đã hoạt động và sẽ tự hiển thị bài mới khi được thêm vào database.</p>
                        <button onClick={() => setSelectedSkill(1)} className="mt-5 rounded-xl bg-white px-5 py-3 text-sm font-bold text-slate-950">Luyện Reading ngay</button>
                    </div>
                )}
            </section>
            <div className="h-20 md:hidden" />
        </LearningShell>
    );
};

export default Practice;
