import { useEffect, useMemo, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import LearningShell from "../../components/LearningShell";
import { apiRequest } from "../../services/api";

const levelNames = { 1: "A1", 2: "A2", 3: "B1", 4: "B2", 5: "C1", 6: "C2" };
const modes = [
    { value: "flashcard", label: "Flashcard", icon: "style", description: "Lật thẻ và tự đánh giá" },
    { value: "choice", label: "Trắc nghiệm", icon: "quiz", description: "Chọn đúng nghĩa tiếng Việt" },
    { value: "typing", label: "Điền từ", icon: "keyboard", description: "Gõ lại từ tiếng Anh" }
];

const shuffleWords = items => {
    const shuffled = [...items];
    for (let index = shuffled.length - 1; index > 0; index--) {
        const swapIndex = Math.floor(Math.random() * (index + 1));
        [shuffled[index], shuffled[swapIndex]] = [shuffled[swapIndex], shuffled[index]];
    }
    return shuffled;
};

const stableHash = value => {
    let hash = 0;
    for (let index = 0; index < value.length; index++) hash = ((hash << 5) - hash) + value.charCodeAt(index);
    return hash;
};

const Vocabulary = () => {
    const navigate = useNavigate();
    const [searchParams, setSearchParams] = useSearchParams();
    const [topics, setTopics] = useState([]);
    const [words, setWords] = useState([]);
    const [wordPool, setWordPool] = useState([]);
    const [initialCount, setInitialCount] = useState(0);
    const [retryQueue, setRetryQueue] = useState([]);
    const [learnedWordIds, setLearnedWordIds] = useState(new Set());
    const [round, setRound] = useState(1);
    const [profile, setProfile] = useState(null);
    const [index, setIndex] = useState(0);
    const [mode, setMode] = useState("flashcard");
    const [revealed, setRevealed] = useState(false);
    const [typedAnswer, setTypedAnswer] = useState("");
    const [selectedAnswer, setSelectedAnswer] = useState("");
    const [answerState, setAnswerState] = useState(null);
    const [loading, setLoading] = useState(true);
    const [reviewing, setReviewing] = useState(false);
    const [error, setError] = useState("");
    const [sessionXp, setSessionXp] = useState(0);
    const [reviewed, setReviewed] = useState(0);
    const [correctAnswers, setCorrectAnswers] = useState(0);
    const [dictionaryEntries, setDictionaryEntries] = useState({});
    const [discoveryQuery, setDiscoveryQuery] = useState("");
    const [suggestions, setSuggestions] = useState([]);
    const [discoveryLoading, setDiscoveryLoading] = useState(false);
    const [discoveredEntry, setDiscoveredEntry] = useState(null);
    const [discoveryError, setDiscoveryError] = useState("");
    const [discoveryMessage, setDiscoveryMessage] = useState("");

    const topic = searchParams.get("topic") || "";
    const level = searchParams.get("level") || (profile ? String(profile.currentLevel) : "");
    const currentWord = words[index];
    const dictionaryEntry = currentWord ? dictionaryEntries[currentWord.word.toLowerCase()] : null;
    const displayWord = currentWord ? {
        ...currentWord,
        pronunciation: dictionaryEntry?.phonetic || currentWord.pronunciation,
        partOfSpeech: dictionaryEntry?.partOfSpeech || currentWord.partOfSpeech,
        meaning: dictionaryEntry?.definition || currentWord.meaning,
        exampleSentence: currentWord.exampleSentence || dictionaryEntry?.example || ""
    } : null;
    const progressPercent = initialCount ? Math.round((learnedWordIds.size / initialCount) * 100) : 0;

    const choiceOptions = useMemo(() => {
        if (!currentWord) return [];
        const distractors = wordPool
            .filter(word => word.id !== currentWord.id)
            .sort((first, second) => stableHash(`${currentWord.id}:${first.id}:${round}`) - stableHash(`${currentWord.id}:${second.id}:${round}`))
            .slice(0, 3);
        return [currentWord, ...distractors]
            .sort((first, second) => stableHash(`${round}:${currentWord.id}:${first.id}`) - stableHash(`${round}:${currentWord.id}:${second.id}`));
    }, [currentWord, round, wordPool]);

    useEffect(() => {
        if (!localStorage.getItem("token")) {
            navigate("/register");
            return;
        }
        Promise.all([apiRequest("/Learning/topics"), apiRequest("/Learning/profile")])
            .then(([topicData, profileData]) => {
                setTopics(topicData);
                setProfile(profileData);
            })
            .catch(err => setError(err.message));
    }, [navigate]);

    useEffect(() => {
        if (!profile) return;
        const query = new URLSearchParams({ take: "50" });
        if (topic) query.set("topic", topic);
        if (level) query.set("level", level);
        apiRequest(`/Learning/vocabulary?${query}`)
            .then(data => {
                setWordPool(data);
                setWords(shuffleWords(data));
                setInitialCount(data.length);
                setRetryQueue([]);
                setLearnedWordIds(new Set());
                setRound(1);
                setIndex(0);
                setReviewed(0);
                setCorrectAnswers(0);
                setSessionXp(0);
                setRevealed(false);
                setAnswerState(null);
                setTypedAnswer("");
                setSelectedAnswer("");
            })
            .catch(err => setError(err.message))
            .finally(() => setLoading(false));
    }, [profile, topic, level]);

    useEffect(() => {
        if (!currentWord || dictionaryEntries[currentWord.word.toLowerCase()] !== undefined) return;
        const key = currentWord.word.toLowerCase();
        apiRequest(`/Learning/dictionary/${encodeURIComponent(currentWord.word)}`)
            .then(entry => setDictionaryEntries(current => ({ ...current, [key]: entry })))
            .catch(() => setDictionaryEntries(current => ({ ...current, [key]: null })));
    }, [currentWord, dictionaryEntries]);

    const selectedTopic = useMemo(() => topics.find(item => item.slug === topic), [topics, topic]);

    const resetQuestion = () => {
        setRevealed(false);
        setTypedAnswer("");
        setSelectedAnswer("");
        setAnswerState(null);
    };

    const restart = () => {
        setWords(shuffleWords(wordPool));
        setIndex(0);
        setReviewed(0);
        setCorrectAnswers(0);
        setSessionXp(0);
        setRetryQueue([]);
        setLearnedWordIds(new Set());
        setRound(1);
        resetQuestion();
    };

    const changeMode = nextMode => {
        setMode(nextMode);
        restart();
    };

    const changeFilter = (key, value) => {
        setLoading(true);
        const next = new URLSearchParams(searchParams);
        value ? next.set(key, value) : next.delete(key);
        setSearchParams(next);
    };

    const speak = () => {
        if (!currentWord) return;
        if (dictionaryEntry?.audioUrl) {
            new Audio(dictionaryEntry.audioUrl).play().catch(() => {});
            return;
        }
        if (!("speechSynthesis" in window)) return;
        window.speechSynthesis.cancel();
        const utterance = new SpeechSynthesisUtterance(currentWord.word);
        utterance.lang = "en-US";
        utterance.rate = 0.85;
        window.speechSynthesis.speak(utterance);
    };

    const recordAnswer = async isCorrect => {
        if (!currentWord || reviewing || answerState) return;
        setReviewing(true);
        setError("");
        try {
            const result = await apiRequest(`/Learning/vocabulary/${currentWord.id}/review`, {
                method: "POST",
                body: JSON.stringify({ isCorrect })
            });
            const wasStarted = currentWord.hasStarted;
            const wasMastered = currentWord.isMastered;
            const updatedWord = { ...currentWord, hasStarted: true, masteryLevel: result.masteryLevel, isMastered: result.isMastered };
            setSessionXp(value => value + result.experiencePointsEarned);
            setReviewed(value => value + 1);
            if (isCorrect) {
                setCorrectAnswers(value => value + 1);
                setLearnedWordIds(current => new Set([...current, currentWord.id]));
            } else {
                setRetryQueue(current => current.some(word => word.id === currentWord.id) ? current : [...current, updatedWord]);
            }
            setAnswerState({ isCorrect, message: isCorrect ? "Chính xác!" : `Đáp án đúng: ${currentWord.word}` });
            setWords(current => current.map(word => word.id === currentWord.id ? updatedWord : word));
            setWordPool(current => current.map(word => word.id === currentWord.id ? updatedWord : word));
            setTopics(current => current.map(item => item.slug === currentWord.topicSlug ? {
                ...item,
                startedCount: item.startedCount + (wasStarted ? 0 : 1),
                masteredCount: item.masteredCount + (!wasMastered && result.isMastered ? 1 : 0)
            } : item));
        } catch (err) {
            setError(err.message);
        } finally {
            setReviewing(false);
        }
    };

    const goNext = () => {
        if (index < words.length - 1) {
            setIndex(value => value + 1);
            resetQuestion();
        } else if (retryQueue.length > 0) {
            setWords(shuffleWords(retryQueue));
            setRetryQueue([]);
            setIndex(0);
            setRound(value => value + 1);
            resetQuestion();
        } else {
            setIndex(words.length);
        }
    };

    const shuffleRemaining = () => {
        setWords(current => shuffleWords(current.slice(index)));
        setIndex(0);
        resetQuestion();
    };

    const submitTyping = event => {
        event.preventDefault();
        const normalizedAnswer = typedAnswer.trim().toLowerCase();
        recordAnswer(normalizedAnswer === currentWord.word.trim().toLowerCase());
    };

    const discoverWords = async event => {
        event.preventDefault();
        if (!discoveryQuery.trim()) return;
        setDiscoveryLoading(true);
        setDiscoveryError("");
        setDiscoveryMessage("");
        setDiscoveredEntry(null);
        try {
            setSuggestions(await apiRequest(`/Learning/discover?query=${encodeURIComponent(discoveryQuery.trim())}&take=50`));
        } catch (err) {
            setDiscoveryError(err.message);
        } finally {
            setDiscoveryLoading(false);
        }
    };

    const inspectSuggestion = async suggestion => {
        setDiscoveryLoading(true);
        setDiscoveryError("");
        setDiscoveryMessage("");
        try {
            const entry = await apiRequest(`/Learning/dictionary/${encodeURIComponent(suggestion.word)}`);
            setDiscoveredEntry({ ...suggestion, ...entry });
        } catch (err) {
            setDiscoveredEntry(suggestion);
            setDiscoveryError(err.message);
        } finally {
            setDiscoveryLoading(false);
        }
    };

    const addDiscoveredWord = async () => {
        if (!topic) {
            setDiscoveryError("Hãy chọn một chủ đề ở cột bên trái trước khi thêm từ.");
            return;
        }
        if (!discoveredEntry?.meaningVietnamese) {
            setDiscoveryError("Chưa lấy được nghĩa tiếng Việt của từ này.");
            return;
        }
        setDiscoveryLoading(true);
        setDiscoveryError("");
        try {
            const addedWord = await apiRequest("/Learning/vocabulary/discovered", {
                method: "POST",
                body: JSON.stringify({
                    word: discoveredEntry.word,
                    topicSlug: topic,
                    level: Number(level),
                    pronunciation: discoveredEntry.phonetic,
                    partOfSpeech: discoveredEntry.partOfSpeech,
                    meaning: discoveredEntry.definition,
                    meaningVietnamese: discoveredEntry.meaningVietnamese,
                    exampleSentence: discoveredEntry.example
                })
            });
            const alreadyInSession = wordPool.some(word => word.id === addedWord.id);
            if (!alreadyInSession) {
                setWordPool(current => [...current, addedWord]);
                setWords(current => [...current, addedWord]);
                setInitialCount(value => value + 1);
                setTopics(current => current.map(item => item.slug === topic ? { ...item, wordCount: item.wordCount + 1 } : item));
            }
            setDiscoveryMessage(alreadyInSession ? "Từ này đã có trong bộ đang học." : "Đã thêm từ vào bộ và phiên học hiện tại.");
        } catch (err) {
            setDiscoveryError(err.message);
        } finally {
            setDiscoveryLoading(false);
        }
    };

    const renderFlashcard = () => (
        <>
            <div className="mb-5 flex items-center gap-3">
                <h2 className="text-5xl font-black tracking-tight text-white md:text-7xl">{displayWord.word}</h2>
                <button onClick={speak} className="grid h-11 w-11 place-items-center rounded-full bg-cyan-400/10 text-cyan-300 transition hover:bg-cyan-400/20"><span className="material-symbols-outlined">volume_up</span></button>
            </div>
            <div className="flex items-center gap-3 text-sm">{displayWord.pronunciation && <span className="text-cyan-300">{displayWord.pronunciation}</span>}{displayWord.partOfSpeech && <span className="rounded-lg bg-white/5 px-2 py-1 italic text-slate-500">{displayWord.partOfSpeech}</span>}</div>
            {!revealed ? (
                <button onClick={() => setRevealed(true)} className="mt-12 flex items-center gap-2 rounded-2xl bg-white px-7 py-3.5 font-bold text-slate-950 transition hover:-translate-y-0.5"><span className="material-symbols-outlined">visibility</span>Lật thẻ</button>
            ) : (
                <div className="mt-8 w-full max-w-2xl animate-[fadeIn_.25s_ease-out]">
                    <h3 className="text-2xl font-extrabold text-violet-300">{displayWord.meaningVietnamese}</h3>
                    {displayWord.meaning && <p className="mt-2 text-sm text-slate-500">{displayWord.meaning}</p>}
                    {displayWord.exampleSentence && <div className="mt-5 rounded-2xl border border-white/10 bg-white/[.035] p-5 text-left"><p className="font-semibold leading-7 text-slate-200">“{displayWord.exampleSentence}”</p>{displayWord.exampleTranslation && <p className="mt-2 text-sm text-slate-500">{displayWord.exampleTranslation}</p>}</div>}
                </div>
            )}
        </>
    );

    const renderChoice = () => (
        <div className="w-full max-w-2xl">
            <p className="text-xs font-bold uppercase tracking-[.18em] text-slate-500">Chọn nghĩa đúng của từ</p>
            <div className="mt-5 flex items-center justify-center gap-3"><h2 className="text-5xl font-black text-white md:text-6xl">{displayWord.word}</h2><button onClick={speak} className="grid h-10 w-10 place-items-center rounded-full bg-cyan-400/10 text-cyan-300"><span className="material-symbols-outlined">volume_up</span></button></div>
            <div className="mt-9 grid gap-3 sm:grid-cols-2">
                {choiceOptions.map(option => {
                    const isSelected = selectedAnswer === option.id;
                    const isCorrectOption = option.id === currentWord.id;
                    const showCorrect = answerState && isCorrectOption;
                    const showWrong = answerState && isSelected && !isCorrectOption;
                    return <button key={option.id} disabled={Boolean(answerState) || reviewing} onClick={() => { setSelectedAnswer(option.id); recordAnswer(isCorrectOption); }} className={`rounded-2xl border p-4 text-left text-sm font-semibold transition ${showCorrect ? "border-emerald-400 bg-emerald-400/15 text-emerald-200" : showWrong ? "border-rose-400 bg-rose-400/15 text-rose-200" : "border-white/10 bg-white/[.035] text-slate-300 hover:border-violet-400/50 hover:bg-violet-500/10"}`}>{option.meaningVietnamese}</button>;
                })}
            </div>
        </div>
    );

    const renderTyping = () => (
        <form onSubmit={submitTyping} className="w-full max-w-xl">
            <p className="text-xs font-bold uppercase tracking-[.18em] text-slate-500">Điền từ tiếng Anh phù hợp</p>
            <h2 className="mt-5 text-3xl font-black text-violet-300 md:text-4xl">{displayWord.meaningVietnamese}</h2>
            <p className="mt-3 text-sm text-slate-500">{displayWord.partOfSpeech} · {levelNames[displayWord.level]} · {displayWord.word.length} ký tự</p>
            <div className="relative mt-9">
                <input autoFocus value={typedAnswer} onChange={event => setTypedAnswer(event.target.value)} disabled={Boolean(answerState)} autoComplete="off" className={`w-full rounded-2xl border bg-slate-950/70 px-5 py-4 text-center text-xl font-bold tracking-wide text-white outline-none transition placeholder:text-slate-700 ${answerState?.isCorrect ? "border-emerald-400" : answerState ? "border-rose-400" : "border-white/15 focus:border-violet-400 focus:ring-4 focus:ring-violet-500/10"}`} placeholder="Nhập từ tiếng Anh..." />
            </div>
            {!answerState && <button type="submit" disabled={!typedAnswer.trim() || reviewing} className="mt-4 w-full rounded-2xl bg-violet-600 py-3.5 font-bold transition hover:bg-violet-500 disabled:cursor-not-allowed disabled:opacity-40">Kiểm tra đáp án</button>}
        </form>
    );

    return (
        <LearningShell>
            <div className="mb-7 flex flex-col justify-between gap-5 md:flex-row md:items-end">
                <div><div className="mb-2 flex items-center gap-2 text-xs font-bold uppercase tracking-[.2em] text-cyan-400"><span className="material-symbols-outlined text-lg">neurology</span> Học & kiểm tra</div><h1 className="text-3xl font-black tracking-tight md:text-4xl">Từ vựng theo lộ trình</h1><p className="mt-2 text-slate-400">Flashcard, trắc nghiệm và điền từ trên cùng một bộ tiến độ.</p></div>
                <div className="flex gap-3"><div className="rounded-2xl border border-amber-400/15 bg-amber-400/[.06] px-5 py-3 text-center"><p className="text-xs text-slate-500">Phiên này</p><strong className="text-lg text-amber-300">+{sessionXp} XP</strong></div><div className="rounded-2xl border border-violet-400/15 bg-violet-400/[.06] px-5 py-3 text-center"><p className="text-xs text-slate-500">Đã thuộc</p><strong className="text-lg text-violet-300">{learnedWordIds.size}/{initialCount}</strong></div></div>
            </div>

            <div className="mb-7 grid gap-2 rounded-2xl border border-white/10 bg-white/[.025] p-2 sm:grid-cols-3">
                {modes.map(item => <button key={item.value} onClick={() => changeMode(item.value)} className={`flex items-center gap-3 rounded-xl px-4 py-3 text-left transition ${mode === item.value ? "bg-violet-500 text-white shadow-lg shadow-violet-950/30" : "text-slate-400 hover:bg-white/5"}`}><span className="material-symbols-outlined">{item.icon}</span><span><strong className="block text-sm">{item.label}</strong><span className={`text-[10px] ${mode === item.value ? "text-violet-100/70" : "text-slate-600"}`}>{item.description}</span></span></button>)}
            </div>

            <section className="mb-7 rounded-3xl border border-cyan-400/15 bg-cyan-400/[.035] p-5">
                <div className="flex flex-col gap-4 lg:flex-row lg:items-center">
                    <div className="lg:w-72"><div className="flex items-center gap-2 text-sm font-black text-white"><span className="material-symbols-outlined text-cyan-300">travel_explore</span>Kho từ mở rộng</div><p className="mt-1 text-xs text-slate-500">Tìm hàng nghìn từ liên quan bằng Datamuse API.</p></div>
                    <form onSubmit={discoverWords} className="flex flex-1 gap-2"><input value={discoveryQuery} onChange={event => setDiscoveryQuery(event.target.value)} className="min-w-0 flex-1 rounded-xl border border-white/10 bg-slate-950/70 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-700 focus:border-cyan-400" placeholder="Ví dụ: music, business, health..." /><button disabled={discoveryLoading || !discoveryQuery.trim()} className="rounded-xl bg-cyan-400 px-5 py-3 text-sm font-black text-cyan-950 disabled:opacity-40">{discoveryLoading ? "Đang tìm..." : "Khám phá"}</button></form>
                </div>
                {suggestions.length > 0 && <div className="mt-4 flex max-h-32 flex-wrap gap-2 overflow-y-auto border-t border-white/5 pt-4">{suggestions.map(suggestion => <button key={suggestion.word} onClick={() => inspectSuggestion(suggestion)} className="rounded-full border border-white/10 bg-slate-900 px-3 py-1.5 text-xs font-semibold text-slate-400 transition hover:border-cyan-400/40 hover:text-cyan-300">{suggestion.word}</button>)}</div>}
                {discoveredEntry && <div className="mt-4 flex flex-col justify-between gap-4 rounded-2xl border border-white/10 bg-slate-950/60 p-4 lg:flex-row lg:items-center"><div><div className="flex flex-wrap items-center gap-2"><strong className="text-xl text-white">{discoveredEntry.word}</strong>{discoveredEntry.phonetic && <span className="text-sm text-cyan-300">{discoveredEntry.phonetic}</span>}{discoveredEntry.partOfSpeech && <span className="rounded bg-white/5 px-2 py-1 text-[10px] text-slate-500">{discoveredEntry.partOfSpeech}</span>}</div>{discoveredEntry.meaningVietnamese && <p className="mt-2 font-bold text-violet-300">{discoveredEntry.meaningVietnamese}</p>}<p className="mt-1 text-sm text-slate-400">{discoveredEntry.definition || "Từ liên quan được tìm thấy trong kho mở rộng."}</p></div><div className="flex shrink-0 gap-2">{discoveredEntry.audioUrl && <button onClick={() => new Audio(discoveredEntry.audioUrl).play()} className="grid h-10 w-10 place-items-center rounded-xl bg-cyan-400/10 text-cyan-300"><span className="material-symbols-outlined">volume_up</span></button>}<button onClick={addDiscoveredWord} disabled={discoveryLoading || !discoveredEntry.meaningVietnamese} className="rounded-xl bg-violet-600 px-4 py-2 text-xs font-bold text-white disabled:opacity-40">Thêm vào bộ đang học</button></div></div>}
                {discoveryError && <p className="mt-3 text-xs text-rose-300">{discoveryError}</p>}
                {discoveryMessage && <p className="mt-3 text-xs font-semibold text-emerald-300">{discoveryMessage}</p>}
            </section>

            {error && <div className="mb-6 flex items-center gap-3 rounded-2xl border border-rose-500/20 bg-rose-500/10 p-4 text-sm text-rose-300"><span className="material-symbols-outlined">error</span>{error}</div>}

            <div className="grid gap-7 lg:grid-cols-[280px_1fr]">
                <aside className="space-y-5">
                    <div className="rounded-3xl border border-white/10 bg-white/[.035] p-5"><label className="mb-3 block text-xs font-bold uppercase tracking-wider text-slate-500">Trình độ</label><div className="grid grid-cols-3 gap-2">{Object.entries(levelNames).map(([value, label]) => <button key={value} onClick={() => changeFilter("level", value)} className={`rounded-xl border py-2.5 text-sm font-black transition ${String(level) === value ? "border-violet-400/60 bg-violet-500/15 text-violet-300" : "border-white/10 text-slate-500 hover:text-white"}`}>{label}</button>)}</div></div>
                    <div className="rounded-3xl border border-white/10 bg-white/[.035] p-5"><label className="mb-3 block text-xs font-bold uppercase tracking-wider text-slate-500">Chủ đề</label><div className="space-y-1.5"><button onClick={() => changeFilter("topic", "")} className={`flex w-full items-center gap-3 rounded-xl px-3 py-3 text-left text-sm font-semibold transition ${!topic ? "bg-cyan-400/10 text-cyan-300" : "text-slate-400 hover:bg-white/5"}`}><span className="material-symbols-outlined text-lg">apps</span>Tất cả chủ đề</button>{topics.map(item => { const topicProgress = item.wordCount ? Math.round(item.startedCount / item.wordCount * 100) : 0; return <button key={item.id} onClick={() => changeFilter("topic", item.slug)} className={`w-full rounded-xl px-3 py-2.5 text-left transition ${topic === item.slug ? "bg-cyan-400/10" : "hover:bg-white/5"}`}><span className="flex items-center gap-3"><span className="material-symbols-outlined text-lg text-slate-500">{item.icon}</span><span className="flex-1 truncate text-sm font-semibold text-slate-400">{item.name}</span><span className="text-[10px] text-slate-600">{item.startedCount}/{item.wordCount}</span></span><span className="mt-2 block h-1 overflow-hidden rounded-full bg-slate-800"><span className="block h-full rounded-full bg-cyan-400 transition-all duration-500" style={{ width: `${topicProgress}%` }} /></span></button>; })}</div></div>
                    <div className="rounded-3xl border border-emerald-400/15 bg-emerald-400/[.05] p-5"><div className="flex items-center justify-between text-sm"><span className="font-semibold text-slate-300">Từ đã trả lời đúng</span><strong className="text-emerald-300">{progressPercent}%</strong></div><div className="mt-3 h-2 overflow-hidden rounded-full bg-slate-800"><div className="h-full rounded-full bg-gradient-to-r from-emerald-500 to-cyan-400 transition-all duration-500" style={{ width: `${progressPercent}%` }} /></div><p className="mt-2 text-[11px] text-slate-600">{learnedWordIds.size}/{initialCount} từ · Vòng {round}{retryQueue.length > 0 ? ` · ${retryQueue.length} từ cần ôn lại` : ""}</p></div>
                </aside>

                <section>
                    {loading ? <div className="grid min-h-[540px] place-items-center rounded-[2rem] border border-white/10 bg-white/[.025]"><span className="material-symbols-outlined animate-spin text-4xl text-violet-400">progress_activity</span></div> : words.length === 0 ? <div className="grid min-h-[540px] place-items-center rounded-[2rem] border border-dashed border-white/15 bg-white/[.025] p-8 text-center"><div><span className="material-symbols-outlined text-6xl text-slate-700">dictionary</span><h2 className="mt-4 text-xl font-bold">Chưa có từ cho bộ lọc này</h2></div></div> : index >= words.length ? (
                        <div className="grid min-h-[540px] place-items-center rounded-[2rem] border border-emerald-400/20 bg-gradient-to-br from-emerald-500/10 to-cyan-500/5 p-8 text-center"><div className="max-w-md"><span className="mx-auto grid h-20 w-20 place-items-center rounded-full bg-emerald-400/15 text-emerald-300"><span className="material-symbols-outlined text-5xl">celebration</span></span><p className="mt-6 text-xs font-bold uppercase tracking-[.2em] text-emerald-400">Hoàn thành {modes.find(item => item.value === mode)?.label}</p><h2 className="mt-3 text-3xl font-black">Đúng {correctAnswers}/{reviewed} câu</h2><p className="mt-3 text-slate-400">Tiến độ đã được lưu và lịch ôn tiếp theo đã được cập nhật.</p><div className="mt-7 flex justify-center gap-3"><button onClick={() => navigate("/dashboard")} className="rounded-xl border border-white/10 px-5 py-3 text-sm font-bold text-slate-300 hover:bg-white/5">Về lộ trình</button><button onClick={restart} className="rounded-xl bg-emerald-500 px-5 py-3 text-sm font-bold text-emerald-950 hover:bg-emerald-400">Làm lại</button></div></div></div>
                    ) : (
                        <div className="relative min-h-[540px] overflow-hidden rounded-[2rem] border border-white/10 bg-gradient-to-br from-slate-900 to-[#10162a] p-6 shadow-2xl shadow-black/20 md:p-10">
                            <div className="absolute right-0 top-0 h-64 w-64 rounded-full bg-violet-600/10 blur-3xl" />
                            <div className="relative flex items-center justify-between"><div className="flex items-center gap-2"><span className="rounded-full bg-violet-500/15 px-3 py-1 text-xs font-bold text-violet-300">{levelNames[currentWord.level]}</span><span className="text-xs text-slate-500">{currentWord.topicName || selectedTopic?.name}</span></div><div className="flex items-center gap-2"><button disabled={Boolean(answerState)} onClick={shuffleRemaining} className="flex items-center gap-1 rounded-lg bg-white/5 px-2.5 py-1.5 text-[10px] font-bold text-slate-400 hover:text-cyan-300 disabled:opacity-30"><span className="material-symbols-outlined text-sm">shuffle</span>Xáo trộn</button><span className="text-sm font-semibold text-slate-500">{index + 1} / {words.length}</span></div></div>
                            <div className="relative flex min-h-[390px] flex-col items-center justify-center text-center">{mode === "flashcard" && renderFlashcard()}{mode === "choice" && renderChoice()}{mode === "typing" && renderTyping()}</div>
                            {mode === "flashcard" && revealed && !answerState && <div className="relative grid gap-3 border-t border-white/10 pt-5 sm:grid-cols-2"><button disabled={reviewing} onClick={() => recordAnswer(false)} className="flex items-center justify-center gap-2 rounded-2xl border border-rose-400/25 bg-rose-500/[.07] py-3.5 font-bold text-rose-300 hover:bg-rose-500/15"><span className="material-symbols-outlined">refresh</span>Chưa nhớ</button><button disabled={reviewing} onClick={() => recordAnswer(true)} className="flex items-center justify-center gap-2 rounded-2xl bg-gradient-to-r from-emerald-500 to-cyan-500 py-3.5 font-bold text-slate-950"><span className="material-symbols-outlined">check_circle</span>Đã nhớ</button></div>}
                            {answerState && <div className={`relative flex flex-col items-center justify-between gap-3 rounded-2xl border p-3 sm:flex-row ${answerState.isCorrect ? "border-emerald-400/20 bg-emerald-400/10 text-emerald-300" : "border-rose-400/20 bg-rose-400/10 text-rose-300"}`}><span className="flex items-center gap-2 text-sm font-bold"><span className="material-symbols-outlined">{answerState.isCorrect ? "check_circle" : "cancel"}</span>{answerState.message}</span><button onClick={goNext} className="flex items-center gap-1 rounded-xl bg-white px-4 py-2 text-sm font-bold text-slate-950">Câu tiếp theo <span className="material-symbols-outlined text-lg">arrow_forward</span></button></div>}
                        </div>
                    )}
                </section>
            </div>
            <div className="h-20 md:hidden" />
        </LearningShell>
    );
};

export default Vocabulary;
