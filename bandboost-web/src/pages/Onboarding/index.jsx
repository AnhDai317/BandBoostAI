import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiRequest } from "../../services/api";
import GoalDatePicker from "../../components/GoalDatePicker";

const goals = [
    { value: 2, title: "Chinh phục IELTS", description: "Lộ trình theo band điểm và 4 kỹ năng", icon: "workspace_premium" },
    { value: 3, title: "Giao tiếp tự tin", description: "Phản xạ tốt hơn trong tình huống thực tế", icon: "forum" },
    { value: 4, title: "Phát triển sự nghiệp", description: "Tiếng Anh chuyên nghiệp cho công việc", icon: "business_center" },
    { value: 6, title: "Du học", description: "Sẵn sàng cho môi trường học thuật quốc tế", icon: "school" },
    { value: 5, title: "Du lịch", description: "Giao tiếp chủ động trong mọi hành trình", icon: "flight_takeoff" },
    { value: 1, title: "Tiếng Anh tổng quát", description: "Phát triển cân bằng và bền vững", icon: "auto_stories" }
];

const levels = [
    { value: 1, code: "A1", label: "Mới bắt đầu", detail: "Hiểu từ và câu rất cơ bản" },
    { value: 2, code: "A2", label: "Sơ cấp", detail: "Giao tiếp trong tình huống quen thuộc" },
    { value: 3, code: "B1", label: "Trung cấp", detail: "Xử lý phần lớn tình huống hằng ngày" },
    { value: 4, code: "B2", label: "Trung cao cấp", detail: "Giao tiếp khá tự nhiên và chi tiết" },
    { value: 5, code: "C1", label: "Cao cấp", detail: "Sử dụng linh hoạt cho học tập, công việc" },
    { value: 6, code: "C2", label: "Thành thạo", detail: "Hiểu và diễn đạt gần như người bản ngữ" }
];

const defaultDate = () => {
    const date = new Date();
    date.setDate(date.getDate() + 90);
    return date.toISOString().slice(0, 10);
};

const Onboarding = () => {
    const navigate = useNavigate();
    const [step, setStep] = useState(1);
    const [topics, setTopics] = useState([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [form, setForm] = useState({
        primaryGoal: 2,
        currentLevel: 2,
        targetLevel: 4,
        targetBandScore: 7,
        dailyMinutes: 20,
        weeklyGoalDays: 5,
        targetDate: defaultDate(),
        preferredTopics: []
    });

    useEffect(() => {
        if (!localStorage.getItem("token")) {
            navigate("/register");
            return;
        }

        Promise.all([apiRequest("/Learning/profile"), apiRequest("/Learning/topics")])
            .then(([profile, topicData]) => {
                setTopics(topicData);
                if (profile.isOnboardingCompleted) {
                    setForm({
                        primaryGoal: profile.primaryGoal,
                        currentLevel: profile.currentLevel,
                        targetLevel: profile.targetLevel,
                        targetBandScore: profile.targetBandScore || 7,
                        dailyMinutes: profile.dailyMinutes,
                        weeklyGoalDays: profile.weeklyGoalDays,
                        targetDate: profile.targetDate?.slice(0, 10) || defaultDate(),
                        preferredTopics: profile.preferredTopics || []
                    });
                }
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [navigate]);

    const selectedGoal = useMemo(() => goals.find(goal => goal.value === form.primaryGoal), [form.primaryGoal]);

    const chooseTopic = slug => {
        setForm(current => ({
            ...current,
            preferredTopics: current.preferredTopics.includes(slug)
                ? current.preferredTopics.filter(item => item !== slug)
                : current.preferredTopics.length < 5
                    ? [...current.preferredTopics, slug]
                    : current.preferredTopics
        }));
    };

    const goNext = () => {
        if (step === 2 && form.targetLevel < form.currentLevel) {
            setError("Trình độ mục tiêu nên bằng hoặc cao hơn trình độ hiện tại.");
            return;
        }
        setError("");
        setStep(current => Math.min(3, current + 1));
    };

    const saveProfile = async () => {
        if (!form.preferredTopics.length) {
            setError("Hãy chọn ít nhất một chủ đề bạn quan tâm.");
            return;
        }

        setSaving(true);
        setError("");
        try {
            await apiRequest("/Learning/profile", {
                method: "PUT",
                body: JSON.stringify({ ...form, targetBandScore: form.primaryGoal === 2 ? Number(form.targetBandScore) : 0 })
            });
            navigate("/dashboard", { replace: true });
        } catch (err) {
            setError(err.message);
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return <div className="grid min-h-screen place-items-center bg-[#080d1b] text-violet-300"><span className="material-symbols-outlined animate-spin text-4xl">progress_activity</span></div>;
    }

    return (
        <div className="min-h-screen bg-[#080d1b] text-slate-100">
            <div className="pointer-events-none fixed inset-0 bg-[radial-gradient(circle_at_15%_10%,rgba(124,58,237,.18),transparent_30%),radial-gradient(circle_at_90%_80%,rgba(37,99,235,.12),transparent_30%)]" />
            <header className="relative mx-auto flex max-w-6xl items-center justify-between px-5 py-6">
                <button onClick={() => navigate("/dashboard")} className="flex items-center gap-3 text-lg font-extrabold">
                    <span className="grid h-10 w-10 place-items-center rounded-2xl bg-gradient-to-br from-violet-500 to-indigo-600"><span className="material-symbols-outlined">graphic_eq</span></span>
                    BandBoost AI
                </button>
                <span className="text-sm font-semibold text-slate-500">Bước {step} / 3</span>
            </header>

            <main className="relative mx-auto max-w-5xl px-5 pb-16 pt-6">
                <div className="mb-10 grid grid-cols-3 gap-2">
                    {[1, 2, 3].map(item => <div key={item} className={`h-1.5 rounded-full transition-all ${item <= step ? "bg-gradient-to-r from-violet-500 to-indigo-500" : "bg-slate-800"}`} />)}
                </div>

                <div className="mb-8 max-w-2xl">
                    <p className="mb-3 text-xs font-bold uppercase tracking-[.22em] text-violet-400">Thiết kế lộ trình riêng cho bạn</p>
                    <h1 className="text-3xl font-black tracking-tight md:text-5xl">
                        {step === 1 && "Bạn học tiếng Anh để làm gì?"}
                        {step === 2 && "Bạn đang ở đâu trên hành trình?"}
                        {step === 3 && "Biến mục tiêu thành thói quen"}
                    </h1>
                    <p className="mt-4 text-slate-400">
                        {step === 1 && "Mục tiêu giúp hệ thống ưu tiên đúng nội dung và kỹ năng quan trọng nhất."}
                        {step === 2 && "Chọn mức gần nhất với khả năng hiện tại. Bạn luôn có thể điều chỉnh sau."}
                        {step === 3 && "Một lịch học vừa sức sẽ hiệu quả hơn những buổi học dài nhưng không đều."}
                    </p>
                </div>

                {error && <div className="mb-6 flex items-center gap-3 rounded-2xl border border-rose-500/20 bg-rose-500/10 p-4 text-sm text-rose-300"><span className="material-symbols-outlined">error</span>{error}</div>}

                {step === 1 && (
                    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                        {goals.map(goal => (
                            <button key={goal.value} onClick={() => setForm(current => ({ ...current, primaryGoal: goal.value }))} className={`group rounded-3xl border p-6 text-left transition-all ${form.primaryGoal === goal.value ? "border-violet-400/70 bg-violet-500/15 shadow-xl shadow-violet-950/30" : "border-white/10 bg-white/[.035] hover:border-white/20 hover:bg-white/[.06]"}`}>
                                <span className={`mb-5 grid h-12 w-12 place-items-center rounded-2xl ${form.primaryGoal === goal.value ? "bg-violet-500 text-white" : "bg-slate-800 text-slate-400 group-hover:text-violet-300"}`}><span className="material-symbols-outlined">{goal.icon}</span></span>
                                <h3 className="font-bold text-white">{goal.title}</h3>
                                <p className="mt-2 text-sm leading-6 text-slate-400">{goal.description}</p>
                            </button>
                        ))}
                    </div>
                )}

                {step === 2 && (
                    <div className="grid gap-7 lg:grid-cols-2">
                        <section className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                            <h2 className="mb-5 font-bold">Trình độ hiện tại</h2>
                            <div className="space-y-2">
                                {levels.map(level => (
                                    <button key={level.value} onClick={() => setForm(current => ({ ...current, currentLevel: level.value, targetLevel: Math.max(current.targetLevel, level.value) }))} className={`flex w-full items-center gap-4 rounded-2xl border p-3.5 text-left transition ${form.currentLevel === level.value ? "border-violet-400/60 bg-violet-500/15" : "border-transparent bg-slate-900/60 hover:bg-slate-800"}`}>
                                        <span className={`grid h-11 w-11 shrink-0 place-items-center rounded-xl font-black ${form.currentLevel === level.value ? "bg-violet-500" : "bg-slate-800 text-slate-300"}`}>{level.code}</span>
                                        <span><strong className="block text-sm">{level.label}</strong><span className="text-xs text-slate-500">{level.detail}</span></span>
                                    </button>
                                ))}
                            </div>
                        </section>
                        <section className="space-y-5">
                            <div className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                                <h2 className="mb-4 font-bold">Trình độ muốn đạt</h2>
                                <div className="grid grid-cols-3 gap-2">
                                    {levels.filter(level => level.value >= form.currentLevel).map(level => (
                                        <button key={level.value} onClick={() => setForm(current => ({ ...current, targetLevel: level.value }))} className={`rounded-2xl border px-4 py-4 font-black transition ${form.targetLevel === level.value ? "border-cyan-400/60 bg-cyan-400/10 text-cyan-300" : "border-white/10 bg-slate-900/50 text-slate-400"}`}>{level.code}</button>
                                    ))}
                                </div>
                            </div>
                            {form.primaryGoal === 2 && (
                                <div className="rounded-3xl border border-amber-400/20 bg-amber-400/[.06] p-6">
                                    <label className="flex items-center justify-between font-bold"><span>Mục tiêu IELTS</span><strong className="text-3xl text-amber-300">{Number(form.targetBandScore).toFixed(1)}</strong></label>
                                    <input type="range" min="4" max="9" step="0.5" value={form.targetBandScore} onChange={event => setForm(current => ({ ...current, targetBandScore: event.target.value }))} className="mt-5 w-full accent-amber-400" />
                                    <div className="mt-2 flex justify-between text-xs text-slate-500"><span>4.0</span><span>9.0</span></div>
                                </div>
                            )}
                            <div className="rounded-3xl border border-white/10 bg-slate-900/60 p-6">
                                <p className="text-sm text-slate-400">Lộ trình đề xuất</p>
                                <p className="mt-2 font-bold text-white">{levels.find(level => level.value === form.currentLevel)?.code} → {levels.find(level => level.value === form.targetLevel)?.code} · {selectedGoal?.title}</p>
                            </div>
                        </section>
                    </div>
                )}

                {step === 3 && (
                    <div className="grid gap-7 lg:grid-cols-5">
                        <section className="space-y-5 lg:col-span-2">
                            <div className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                                <label className="font-bold">Thời gian mỗi ngày</label>
                                <div className="mt-4 grid grid-cols-3 gap-2">
                                    {[10, 20, 30, 45, 60, 90].map(minutes => <button key={minutes} onClick={() => setForm(current => ({ ...current, dailyMinutes: minutes }))} className={`rounded-xl border py-3 text-sm font-bold ${form.dailyMinutes === minutes ? "border-violet-400 bg-violet-500/15 text-violet-300" : "border-white/10 text-slate-400"}`}>{minutes} phút</button>)}
                                </div>
                            </div>
                            <div className="rounded-3xl border border-white/10 bg-white/[.035] p-6">
                                <label className="font-bold">Số ngày mỗi tuần</label>
                                <div className="mt-4 flex gap-2">
                                    {[3, 4, 5, 6, 7].map(days => <button key={days} onClick={() => setForm(current => ({ ...current, weeklyGoalDays: days }))} className={`grid h-11 flex-1 place-items-center rounded-xl border font-bold ${form.weeklyGoalDays === days ? "border-cyan-400 bg-cyan-400/10 text-cyan-300" : "border-white/10 text-slate-500"}`}>{days}</button>)}
                                </div>
                                <label className="mt-6 block text-sm font-semibold text-slate-300">Ngày muốn đạt mục tiêu</label>
                                <GoalDatePicker value={form.targetDate} onChange={targetDate => setForm(current => ({ ...current, targetDate }))} />
                            </div>
                        </section>
                        <section className="rounded-3xl border border-white/10 bg-white/[.035] p-6 lg:col-span-3">
                            <div className="flex items-end justify-between gap-4">
                                <div><h2 className="font-bold">Chủ đề bạn quan tâm</h2><p className="mt-1 text-sm text-slate-500">Chọn 1–5 chủ đề để cá nhân hóa bài học.</p></div>
                                <span className="text-xs font-bold text-violet-300">{form.preferredTopics.length}/5</span>
                            </div>
                            <div className="mt-5 grid gap-3 sm:grid-cols-2">
                                {topics.map(topic => {
                                    const selected = form.preferredTopics.includes(topic.slug);
                                    return <button key={topic.id} onClick={() => chooseTopic(topic.slug)} className={`flex items-center gap-4 rounded-2xl border p-4 text-left transition ${selected ? "border-violet-400/60 bg-violet-500/15" : "border-white/10 bg-slate-900/50 hover:border-white/20"}`}><span className={`grid h-11 w-11 shrink-0 place-items-center rounded-xl ${selected ? "bg-violet-500 text-white" : "bg-slate-800 text-slate-400"}`}><span className="material-symbols-outlined">{topic.icon}</span></span><span><strong className="block text-sm">{topic.name}</strong><span className="line-clamp-1 text-xs text-slate-500">{topic.description}</span></span>{selected && <span className="material-symbols-outlined ml-auto text-violet-300">check_circle</span>}</button>;
                                })}
                            </div>
                        </section>
                    </div>
                )}

                <div className="mt-10 flex items-center justify-between border-t border-white/10 pt-6">
                    <button onClick={() => step > 1 ? setStep(current => current - 1) : navigate("/dashboard")} className="flex items-center gap-2 rounded-xl px-4 py-3 text-sm font-bold text-slate-400 transition hover:bg-white/5 hover:text-white"><span className="material-symbols-outlined text-lg">arrow_back</span>{step > 1 ? "Quay lại" : "Để sau"}</button>
                    {step < 3 ? (
                        <button onClick={goNext} className="flex items-center gap-2 rounded-2xl bg-gradient-to-r from-violet-600 to-indigo-600 px-7 py-3.5 font-bold shadow-lg shadow-violet-950/50 transition hover:-translate-y-0.5">Tiếp tục <span className="material-symbols-outlined">arrow_forward</span></button>
                    ) : (
                        <button onClick={saveProfile} disabled={saving} className="flex items-center gap-2 rounded-2xl bg-gradient-to-r from-violet-600 to-indigo-600 px-7 py-3.5 font-bold shadow-lg shadow-violet-950/50 transition hover:-translate-y-0.5 disabled:opacity-60">{saving ? "Đang tạo lộ trình..." : "Bắt đầu lộ trình"}<span className={`material-symbols-outlined ${saving ? "animate-spin" : ""}`}>{saving ? "progress_activity" : "rocket_launch"}</span></button>
                    )}
                </div>
            </main>
        </div>
    );
};

export default Onboarding;
