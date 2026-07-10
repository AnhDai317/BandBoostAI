import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import LearningShell from "../../components/LearningShell";
import { apiRequest } from "../../services/api";

const levelNames = { 1: "A1", 2: "A2", 3: "B1", 4: "B2", 5: "C1", 6: "C2" };
const goalNames = { 1: "Tiếng Anh tổng quát", 2: "IELTS", 3: "Giao tiếp", 4: "Sự nghiệp", 5: "Du lịch", 6: "Du học" };
const categoryNames = { 1: "Reading", 2: "Listening", 3: "Writing", 4: "Speaking" };
const categoryStyles = {
    1: "border-sky-400/20 bg-sky-400/10 text-sky-300",
    2: "border-emerald-400/20 bg-emerald-400/10 text-emerald-300",
    3: "border-amber-400/20 bg-amber-400/10 text-amber-300",
    4: "border-rose-400/20 bg-rose-400/10 text-rose-300"
};

const Dashboard = () => {
    const navigate = useNavigate();
    const user = JSON.parse(localStorage.getItem("user") || "{}");
    const [dashboard, setDashboard] = useState(null);
    const [exams, setExams] = useState([]);
    const [attempts, setAttempts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!localStorage.getItem("token")) {
            navigate("/register");
            return;
        }

        Promise.all([
            apiRequest("/Learning/dashboard"),
            apiRequest("/Exams"),
            apiRequest("/Exams/my-attempts")
        ])
            .then(([learningData, examData, attemptData]) => {
                if (learningData.requiresOnboarding) {
                    navigate("/onboarding", { replace: true });
                    return;
                }
                setDashboard(learningData);
                setExams(examData);
                setAttempts(attemptData);
            })
            .catch(err => {
                if (err.status === 401) navigate("/register");
                else setError(err.message);
            })
            .finally(() => setLoading(false));
    }, [navigate]);

    const firstName = user.fullName?.trim().split(" ").pop() || "bạn";
    const dateLabel = new Intl.DateTimeFormat("vi-VN", { weekday: "long", day: "2-digit", month: "long" }).format(new Date());
    const todayProgress = useMemo(() => dashboard ? Math.min(100, Math.round(dashboard.stats.todayMinutes / Math.max(1, dashboard.stats.dailyMinutesTarget) * 100)) : 0, [dashboard]);

    if (loading || !dashboard) {
        return <div className="grid min-h-screen place-items-center bg-[#080d1b] text-violet-300"><div className="text-center"><span className="material-symbols-outlined animate-spin text-5xl">progress_activity</span><p className="mt-3 text-sm font-semibold text-slate-500">Đang chuẩn bị lộ trình hôm nay...</p>{error && <p className="mt-3 text-rose-300">{error}</p>}</div></div>;
    }

    const { profile, stats, todayPlan, milestones, recommendedTopics } = dashboard;
    const targetTitle = profile.primaryGoal === 2 ? `IELTS ${Number(profile.targetBandScore).toFixed(1)}` : `${levelNames[profile.targetLevel]} · ${goalNames[profile.primaryGoal]}`;
    const daysToTarget = profile.targetDate ? Math.max(0, Math.ceil((new Date(profile.targetDate) - new Date()) / 86400000)) : null;

    return (
        <LearningShell>
            {error && <div className="mb-6 flex items-center gap-3 rounded-2xl border border-rose-500/20 bg-rose-500/10 p-4 text-sm text-rose-300"><span className="material-symbols-outlined">error</span>{error}</div>}

            <section className="relative mb-8 overflow-hidden rounded-[2rem] border border-violet-400/15 bg-gradient-to-br from-violet-950/70 via-slate-900 to-indigo-950/70 p-7 md:p-10">
                <div className="pointer-events-none absolute -right-16 -top-28 h-80 w-80 rounded-full bg-violet-500/20 blur-3xl" />
                <div className="relative grid gap-8 lg:grid-cols-[1fr_auto] lg:items-center">
                    <div>
                        <div className="mb-4 flex flex-wrap items-center gap-2 text-xs font-semibold text-slate-400"><span className="capitalize">{dateLabel}</span><span className="h-1 w-1 rounded-full bg-slate-600" /><span>{stats.currentStreak > 0 ? `🔥 ${stats.currentStreak} ngày liên tiếp` : "Bắt đầu streak hôm nay"}</span></div>
                        <h1 className="text-3xl font-black leading-tight tracking-tight md:text-5xl">Chào {firstName},<br /><span className="bg-gradient-to-r from-violet-300 via-fuchsia-300 to-cyan-300 bg-clip-text text-transparent">tiếp tục tiến bộ nhé!</span></h1>
                        <p className="mt-4 max-w-xl text-sm leading-6 text-slate-400 md:text-base">Lộ trình hôm nay được cá nhân hóa cho trình độ <strong className="text-slate-200">{levelNames[profile.currentLevel]}</strong> và mục tiêu <strong className="text-slate-200">{targetTitle}</strong>.</p>
                        <div className="mt-6 flex flex-wrap gap-3">
                            <Link to={todayPlan.find(task => !task.isCompleted)?.route || "/vocabulary"} className="flex items-center gap-2 rounded-2xl bg-white px-6 py-3 font-bold text-slate-950 shadow-lg transition hover:-translate-y-0.5"><span className="material-symbols-outlined">play_arrow</span>Tiếp tục học</Link>
                            <Link to="/onboarding" className="flex items-center gap-2 rounded-2xl border border-white/10 bg-white/5 px-5 py-3 text-sm font-bold text-slate-300 transition hover:bg-white/10"><span className="material-symbols-outlined text-lg">tune</span>Điều chỉnh mục tiêu</Link>
                        </div>
                    </div>

                    <div className="flex items-center gap-6 rounded-3xl border border-white/10 bg-black/20 p-5 backdrop-blur md:min-w-[310px]">
                        <div className="relative grid h-28 w-28 shrink-0 place-items-center rounded-full" style={{ background: `conic-gradient(#a78bfa ${todayProgress * 3.6}deg, rgba(255,255,255,.08) 0deg)` }}>
                            <div className="grid h-[90px] w-[90px] place-items-center rounded-full bg-[#131526] text-center"><div><strong className="text-2xl">{todayProgress}%</strong><p className="text-[10px] text-slate-500">Hôm nay</p></div></div>
                        </div>
                        <div><p className="text-xs font-bold uppercase tracking-wider text-slate-500">Mục tiêu ngày</p><p className="mt-2 text-2xl font-black">{stats.todayMinutes}<span className="text-sm text-slate-500">/{stats.dailyMinutesTarget} phút</span></p><p className="mt-2 text-xs leading-5 text-slate-500">{todayProgress >= 100 ? "Bạn đã hoàn thành kế hoạch hôm nay!" : `Còn ${Math.max(0, stats.dailyMinutesTarget - stats.todayMinutes)} phút để hoàn thành.`}</p></div>
                    </div>
                </div>
            </section>

            <section className="mb-8 grid grid-cols-2 gap-3 md:grid-cols-4">
                {[
                    { label: "Streak hiện tại", value: stats.currentStreak, suffix: " ngày", icon: "local_fire_department", color: "text-orange-300", bg: "bg-orange-400/10" },
                    { label: "Từ đã học", value: stats.wordsStarted, suffix: ` · ${stats.wordsMastered} thành thạo`, icon: "spellcheck", color: "text-emerald-300", bg: "bg-emerald-400/10" },
                    { label: "Nhịp học tuần", value: stats.activeDaysThisWeek, suffix: `/${stats.weeklyGoalDays} ngày`, icon: "calendar_month", color: "text-cyan-300", bg: "bg-cyan-400/10" },
                    { label: "Điểm kinh nghiệm", value: stats.experiencePoints, suffix: " XP", icon: "bolt", color: "text-amber-300", bg: "bg-amber-400/10" }
                ].map(item => <div key={item.label} className="rounded-2xl border border-white/[.08] bg-white/[.035] p-4 md:p-5"><div className="flex items-center gap-3"><span className={`grid h-10 w-10 place-items-center rounded-xl ${item.bg} ${item.color}`}><span className="material-symbols-outlined">{item.icon}</span></span><div><p className="text-xs text-slate-500">{item.label}</p><p className="mt-1 text-xl font-black text-white">{item.value}<span className="text-xs font-semibold text-slate-500">{item.suffix}</span></p></div></div></div>)}
            </section>

            <div className="grid gap-8 lg:grid-cols-[1fr_360px]">
                <div className="space-y-9">
                    <section>
                        <div className="mb-5 flex items-end justify-between"><div><p className="text-xs font-bold uppercase tracking-[.18em] text-violet-400">Kế hoạch cá nhân</p><h2 className="mt-2 text-2xl font-black">Hôm nay học gì?</h2></div><span className="text-xs text-slate-500">{todayPlan.filter(task => task.isCompleted).length}/{todayPlan.length} hoàn thành</span></div>
                        <div className="grid gap-4 md:grid-cols-2">
                            {todayPlan.map((task, taskIndex) => (
                                <Link key={task.title} to={task.route} className={`group relative overflow-hidden rounded-3xl border p-6 transition hover:-translate-y-1 ${task.isCompleted ? "border-emerald-400/20 bg-emerald-400/[.05]" : "border-white/10 bg-white/[.035] hover:border-violet-400/30"}`}>
                                    <div className="flex items-start justify-between"><span className={`grid h-12 w-12 place-items-center rounded-2xl ${taskIndex === 0 ? "bg-violet-500/15 text-violet-300" : "bg-cyan-400/10 text-cyan-300"}`}><span className="material-symbols-outlined">{task.isCompleted ? "check" : task.icon}</span></span><span className="rounded-full bg-white/5 px-3 py-1 text-[11px] font-semibold text-slate-500">~{task.estimatedMinutes} phút</span></div>
                                    <h3 className="mt-5 text-lg font-bold group-hover:text-violet-300">{task.title}</h3><p className="mt-2 text-sm leading-6 text-slate-500">{task.description}</p>
                                    <div className="mt-5 flex items-center gap-1 text-xs font-bold text-violet-300">{task.isCompleted ? "Đã hoàn thành" : "Bắt đầu ngay"}<span className="material-symbols-outlined text-base">arrow_forward</span></div>
                                </Link>
                            ))}
                        </div>
                    </section>

                    <section>
                        <div className="mb-5 flex items-end justify-between"><div><p className="text-xs font-bold uppercase tracking-[.18em] text-cyan-400">Học theo sở thích</p><h2 className="mt-2 text-2xl font-black">Chủ đề dành cho bạn</h2></div><Link to="/vocabulary" className="text-xs font-bold text-slate-400 hover:text-white">Xem tất cả →</Link></div>
                        <div className="grid gap-4 sm:grid-cols-3">
                            {recommendedTopics.map((topic, topicIndex) => {
                                const progress = topic.wordCount ? Math.round(topic.masteredCount / topic.wordCount * 100) : 0;
                                const colors = ["from-violet-600/25 to-fuchsia-600/5 text-violet-300", "from-cyan-600/20 to-blue-600/5 text-cyan-300", "from-emerald-600/20 to-teal-600/5 text-emerald-300"];
                                return <Link key={topic.id} to={`/vocabulary?topic=${topic.slug}&level=${profile.currentLevel}`} className={`rounded-3xl border border-white/10 bg-gradient-to-br ${colors[topicIndex]} p-5 transition hover:-translate-y-1 hover:border-white/20`}><span className="grid h-11 w-11 place-items-center rounded-2xl bg-white/10"><span className="material-symbols-outlined">{topic.icon}</span></span><h3 className="mt-5 font-bold text-white">{topic.name}</h3><p className="mt-2 line-clamp-2 text-xs leading-5 text-slate-500">{topic.description}</p><div className="mt-5 flex items-center justify-between text-[11px] text-slate-500"><span>{topic.masteredCount}/{topic.wordCount} từ</span><strong>{progress}%</strong></div><div className="mt-2 h-1.5 rounded-full bg-black/20"><div className="h-full rounded-full bg-current" style={{ width: `${progress}%` }} /></div></Link>;
                            })}
                        </div>
                    </section>

                    <section id="practice" className="scroll-mt-24">
                        <div className="mb-5"><p className="text-xs font-bold uppercase tracking-[.18em] text-amber-400">Luyện tập có phản hồi</p><h2 className="mt-2 text-2xl font-black">Bài luyện phù hợp</h2></div>
                        <div className="grid gap-4 sm:grid-cols-2">
                            {exams.map(exam => <article key={exam.id} className="rounded-3xl border border-white/10 bg-white/[.035] p-5 transition hover:border-white/20"><div className="flex items-center justify-between"><span className={`rounded-full border px-3 py-1 text-[11px] font-bold ${categoryStyles[exam.category] || "border-white/10 text-slate-400"}`}>{categoryNames[exam.category] || "General"}</span><span className="flex items-center gap-1 text-xs text-slate-500"><span className="material-symbols-outlined text-base">schedule</span>{exam.durationInMinutes} phút</span></div><h3 className="mt-5 font-bold leading-6 text-slate-100">{exam.title}</h3><p className="mt-2 line-clamp-2 text-xs leading-5 text-slate-500">{exam.description}</p><Link to={`/exam/${exam.id}`} className="mt-5 flex items-center justify-center gap-2 rounded-xl bg-slate-800 py-3 text-sm font-bold transition hover:bg-violet-600">Làm bài <span className="material-symbols-outlined text-lg">arrow_forward</span></Link></article>)}
                        </div>
                    </section>
                </div>

                <aside className="space-y-7">
                    <section className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                        <div className="flex items-start justify-between"><div><p className="text-xs font-bold uppercase tracking-wider text-slate-500">Đích đến</p><h2 className="mt-2 text-2xl font-black text-white">{targetTitle}</h2></div><span className="grid h-11 w-11 place-items-center rounded-2xl bg-amber-400/10 text-amber-300"><span className="material-symbols-outlined">flag</span></span></div>
                        <div className="mt-5 grid grid-cols-2 gap-3"><div className="rounded-2xl bg-slate-900/70 p-4"><p className="text-[11px] text-slate-500">Hiện tại</p><strong className="mt-1 block text-xl">{profile.primaryGoal === 2 && profile.currentBandScore ? Number(profile.currentBandScore).toFixed(1) : levelNames[profile.currentLevel]}</strong></div><div className="rounded-2xl bg-slate-900/70 p-4"><p className="text-[11px] text-slate-500">Còn lại</p><strong className="mt-1 block text-xl">{daysToTarget !== null ? `${daysToTarget} ngày` : "Chưa đặt"}</strong></div></div>
                    </section>

                    <section className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                        <div className="mb-6 flex items-center justify-between"><h2 className="font-bold">Các cột mốc</h2><span className="material-symbols-outlined text-slate-600">route</span></div>
                        <div className="space-y-6">
                            {milestones.map((milestone, milestoneIndex) => <div key={milestone.title} className="relative pl-9">{milestoneIndex < milestones.length - 1 && <span className="absolute left-[11px] top-7 h-[calc(100%+12px)] w-px bg-slate-800" />}<span className={`absolute left-0 top-0 grid h-6 w-6 place-items-center rounded-full border text-[10px] font-black ${milestone.status === "completed" ? "border-emerald-400 bg-emerald-400 text-emerald-950" : milestone.status === "in_progress" ? "border-violet-400 bg-violet-500/20 text-violet-300" : "border-slate-700 bg-slate-900 text-slate-500"}`}>{milestone.status === "completed" ? <span className="material-symbols-outlined text-sm">check</span> : milestoneIndex + 1}</span><div className="flex justify-between gap-3"><h3 className="text-sm font-bold text-slate-200">{milestone.title}</h3><span className="text-xs font-bold text-slate-500">{milestone.progressPercent}%</span></div><p className="mt-1 text-xs leading-5 text-slate-500">{milestone.description}</p><div className="mt-2 h-1.5 overflow-hidden rounded-full bg-slate-800"><div className="h-full rounded-full bg-gradient-to-r from-violet-500 to-cyan-400" style={{ width: `${milestone.progressPercent}%` }} /></div></div>)}
                        </div>
                    </section>

                    <section className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                        <div className="mb-4 flex items-center justify-between"><h2 className="font-bold">Kết quả gần đây</h2><span className="text-xs text-slate-600">{attempts.length} bài</span></div>
                        {attempts.length === 0 ? <div className="rounded-2xl border border-dashed border-white/10 p-5 text-center text-xs leading-5 text-slate-500">Chưa có kết quả. Hoàn thành bài luyện đầu tiên để AI phân tích điểm mạnh và điểm cần cải thiện.</div> : <div className="space-y-3">{attempts.slice(0, 3).map(attempt => <Link key={attempt.id} to={`/feedback/${attempt.id}`} className="flex items-center gap-3 rounded-2xl bg-slate-900/60 p-3 transition hover:bg-slate-800"><span className="grid h-10 w-10 place-items-center rounded-xl bg-violet-500/10 font-black text-violet-300">{Number(attempt.score).toFixed(1)}</span><span className="min-w-0 flex-1"><strong className="block truncate text-xs text-slate-300">{attempt.examTitle}</strong><span className="text-[10px] text-slate-600">{new Date(attempt.createdAt).toLocaleDateString("vi-VN")}</span></span><span className="material-symbols-outlined text-lg text-slate-600">chevron_right</span></Link>)}</div>}
                    </section>
                </aside>
            </div>
            <div className="h-20 md:hidden" />
        </LearningShell>
    );
};

export default Dashboard;
