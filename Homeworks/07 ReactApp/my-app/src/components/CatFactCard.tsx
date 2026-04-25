interface CatFactCardProps {
    fact: string;
}

function CatFactCard({ fact }: CatFactCardProps) {
    return (
        <div className="cat-fact-card">
            <p>{fact}</p>
        </div>
    );
}

export default CatFactCard;