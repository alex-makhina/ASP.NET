interface ErrorCardProps {
    message: string;
}

function ErrorCard({message}: ErrorCardProps) {
    return (
        <div className="error-card">
            <p>{message}</p>
        </div>
    );
}

export default ErrorCard;