import { useState } from "react";
import ErrorCard from "./ErrorCard";
import CatFactCard from "./CatFactCard";
import './CatFacts.css';

interface CatFact {
    fact: string;
    length: number;
}

interface CatFactsResponse {
    data: CatFact[];
}

function CatFactPage() {
    const [facts, setFacts] = useState<CatFact[] | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(false);

    const onClick = async () => {
        setLoading(true);
        setError(null);
        setFacts(null);

        try {
            const response = await fetch("https://catfact.ninja/facts");

            if(!response.ok) {
                throw new Error(`API error: ${response.status} ${response.statusText}`);
            }

            const result: CatFactsResponse = await response.json();

            if (result.data.length > 0) {
                setFacts(result.data);
            } else {
                throw new Error('No facts returned from API');
            }
        } catch(err) {
            const message = err instanceof Error ? err.message : 'An unknown error occurred';
            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <main>
            <h1>Cat Facts</h1>
            <button type="button" className="fetch-button" onClick={onClick} disabled={loading}>
                {loading ? "Loading..." : "Get Cat Facts"}
            </button>
            
            {facts && facts.map((fact) => <CatFactCard fact={fact.fact} key={fact.fact} />) }
            {error && <ErrorCard message={error} />}
        </main>
    );
}

export default CatFactPage;