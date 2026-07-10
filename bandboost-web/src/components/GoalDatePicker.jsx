import { useMemo, useState } from "react";

const weekDays = ["T2", "T3", "T4", "T5", "T6", "T7", "CN"];
const monthNames = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];

const toInputDate = date => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
};

const GoalDatePicker = ({ value, onChange }) => {
    const selectedDate = useMemo(() => value ? new Date(`${value}T00:00:00`) : null, [value]);
    const [isOpen, setIsOpen] = useState(false);
    const [visibleMonth, setVisibleMonth] = useState(() => selectedDate || new Date());

    const calendarDays = useMemo(() => {
        const year = visibleMonth.getFullYear();
        const month = visibleMonth.getMonth();
        const firstDay = new Date(year, month, 1);
        const offset = (firstDay.getDay() + 6) % 7;
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        return [
            ...Array(offset).fill(null),
            ...Array.from({ length: daysInMonth }, (_, index) => new Date(year, month, index + 1))
        ];
    }, [visibleMonth]);

    const selectQuickDate = months => {
        const date = new Date();
        date.setMonth(date.getMonth() + months);
        onChange(toInputDate(date));
        setVisibleMonth(date);
        setIsOpen(false);
    };

    const selectDate = date => {
        if (date < new Date(new Date().setHours(0, 0, 0, 0))) return;
        onChange(toInputDate(date));
        setIsOpen(false);
    };

    const moveMonth = direction => {
        setVisibleMonth(current => new Date(current.getFullYear(), current.getMonth() + direction, 1));
    };

    const formattedDate = selectedDate
        ? new Intl.DateTimeFormat("vi-VN", { weekday: "long", day: "2-digit", month: "2-digit", year: "numeric" }).format(selectedDate)
        : "Chọn ngày hoàn thành";

    return (
        <div className="relative mt-2">
            <button
                type="button"
                onClick={() => setIsOpen(open => !open)}
                className={`flex w-full items-center gap-4 rounded-2xl border bg-slate-900/80 px-4 py-3.5 text-left transition ${isOpen ? "border-violet-400 ring-4 ring-violet-500/10" : "border-white/10 hover:border-white/20"}`}
            >
                <span className="grid h-11 w-11 shrink-0 place-items-center rounded-xl bg-violet-500/15 text-violet-300">
                    <span className="material-symbols-outlined">calendar_month</span>
                </span>
                <span className="min-w-0 flex-1">
                    <span className="block text-[11px] font-bold uppercase tracking-wider text-slate-500">Ngày hoàn thành dự kiến</span>
                    <span className="mt-1 block truncate text-sm font-bold capitalize text-white">{formattedDate}</span>
                </span>
                <span className={`material-symbols-outlined text-slate-500 transition ${isOpen ? "rotate-180" : ""}`}>expand_more</span>
            </button>

            <div className="mt-3 grid grid-cols-4 gap-2">
                {[1, 3, 6, 12].map(months => (
                    <button key={months} type="button" onClick={() => selectQuickDate(months)} className="rounded-xl border border-white/10 bg-white/[.035] py-2 text-xs font-semibold text-slate-400 transition hover:border-violet-400/40 hover:bg-violet-500/10 hover:text-violet-300">
                        +{months} tháng
                    </button>
                ))}
            </div>

            {isOpen && (
                <div className="absolute left-0 right-0 top-[132px] z-40 rounded-3xl border border-white/10 bg-[#11182a] p-5 shadow-2xl shadow-black/60">
                    <div className="mb-4 flex items-center justify-between">
                        <button type="button" onClick={() => moveMonth(-1)} className="grid h-9 w-9 place-items-center rounded-xl text-slate-400 hover:bg-white/5 hover:text-white"><span className="material-symbols-outlined">chevron_left</span></button>
                        <strong className="text-sm">{monthNames[visibleMonth.getMonth()]} · {visibleMonth.getFullYear()}</strong>
                        <button type="button" onClick={() => moveMonth(1)} className="grid h-9 w-9 place-items-center rounded-xl text-slate-400 hover:bg-white/5 hover:text-white"><span className="material-symbols-outlined">chevron_right</span></button>
                    </div>
                    <div className="grid grid-cols-7 gap-1 text-center">
                        {weekDays.map(day => <span key={day} className="py-2 text-[10px] font-bold text-slate-600">{day}</span>)}
                        {calendarDays.map((date, index) => {
                            if (!date) return <span key={`empty-${index}`} />;
                            const isSelected = selectedDate && toInputDate(date) === toInputDate(selectedDate);
                            const isToday = toInputDate(date) === toInputDate(new Date());
                            const isPast = date < new Date(new Date().setHours(0, 0, 0, 0));
                            return (
                                <button key={date.toISOString()} type="button" disabled={isPast} onClick={() => selectDate(date)} className={`aspect-square rounded-xl text-xs font-semibold transition ${isSelected ? "bg-violet-500 text-white shadow-lg shadow-violet-950" : isToday ? "border border-cyan-400/40 text-cyan-300" : isPast ? "cursor-not-allowed text-slate-800" : "text-slate-300 hover:bg-white/10"}`}>
                                    {date.getDate()}
                                </button>
                            );
                        })}
                    </div>
                </div>
            )}
        </div>
    );
};

export default GoalDatePicker;
